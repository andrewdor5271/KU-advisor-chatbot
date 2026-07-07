# main.py

import asyncio
import sys
import time
import statistics
import grpc
from dotenv import load_dotenv

from grpc_custom import mainapp_llmworker_pb2
from grpc_custom import mainapp_llmworker_pb2_grpc
import pyodbc # TEST

# load_dotenv("/home/user1/.env")

# sys.path.insert(0, "src")
from src.rag_agent import LLMAgent

class LlmWorker(mainapp_llmworker_pb2_grpc.MessageServiceServicer):
    _agent: LLMAgent
    def __init__(self, agent: LLMAgent = None):
        self._agent = agent

    async def GenerateReply(self, request, context):
        LLM_CONTEXT_COUNT = 50
        conv_id = request.conversation_id

        conn = pyodbc.connect(
            "Driver={ODBC Driver 18 for SQL Server};" +
            "Server=(localdb)\\mssqllocaldb;" +
            "Database=aspnet-MainApp-5d5e236b-7012-4b0d-8144-625083f936f3;" +
            "Trusted_Connection=yes;"
        )
        cursor = conn.cursor()

        cursor.execute(f"""
                       SELECT TOP ({LLM_CONTEXT_COUNT}) *
                       FROM Messages
                       WHERE ConversationId = ?
                       ORDER BY MessageId DESC;""",
                       conv_id)
        messages = cursor.fetchall() # the last message is assumed to be the target
        if messages[0].SenderType == 1: # the last message is sent by the bot
            yield mainapp_llmworker_pb2.NewMessageChunkResponse(
                error=mainapp_llmworker_pb2.Error(
                    message="Last message by a bot"
                ),
                conversation_id=conv_id
            )
            return

        message = messages[0].Text
        history = [x.Text for x in messages[1:]]
        full_text = ""
        # self._agent.request(message, history)
        # async
        for token in message: # temporary for testing
            yield mainapp_llmworker_pb2.NewMessageChunkResponse(
                token=mainapp_llmworker_pb2.TokenChunk(
                    text=token
                ),
                conversation_id=conv_id
            )
            full_text += token
        yield mainapp_llmworker_pb2.NewMessageChunkResponse(
            completion=mainapp_llmworker_pb2.Completion(
                full_text=full_text
            ),
            conversation_id = conv_id
        )

async def serve():
    server = grpc.aio.server()

    mainapp_llmworker_pb2_grpc.add_MessageServiceServicer_to_server(
        LlmWorker(),
        server
    )

    server.add_insecure_port("127.0.0.1:50051")

    await server.start()
    await server.wait_for_termination()
    
'''async def run_query(
    agent: LLMAgent, 
    message: str, 
    history: list[str] | None = None, 
    silent: bool = True
):
    start = time.perf_counter()
    
    token_count = 0
    full_response = ""
    
    if not silent:
        print(f"\nUser: {message}\nAI: ", end="")

    # Capture the text while streaming
    async for token in agent.request(message, history):
        token_count += 1
        full_response += token
        if not silent:
            print(token, end="", flush=True)

    if not silent:
        print("\n")

    end = time.perf_counter()

    return {
        "latency": end - start,
        "tokens": token_count,
        "response": full_response, # Return the text for history tracking
    }


async def worker(agent: LLMAgent, id: int, message: str, results: list):
    # Keep stress test workers silent so they don't spam the Jupyter output
    result = await run_query(agent, f"[User {id}] {message}", silent=True)
    results.append(result)
'''

# agent = await LLMAgent.create()
# await agent.close()

asyncio.run(serve())