import asyncio
import os
import simpleobsws
import socket
import sys
import time

HOST = '127.0.0.1'
OBSPORT = 4567
CPORT = 50005
PASSWORD = '0000'

folder = os.getcwd() + '\\Video'
print(folder)

ws = simpleobsws.obsws(host=HOST, port=OBSPORT, password=PASSWORD)
loop = asyncio.get_event_loop()

async def makeRequest(req):
    print('OBS socket: ' + req)
    await ws.connect()
    res = await ws.call(req)
    print(res)
    await ws.disconnect()

async def makeRequestWithParam(req, param):
    print('OBS socket: ' + req)
    await ws.connect()
    res = await ws.call(req, param)
    print(res)
    await ws.disconnect()

loop.run_until_complete(makeRequestWithParam('SetRecordingFolder', {
            'rec-folder': folder,
        }
    ))

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((HOST, CPORT))
server.listen(1)
print('Server starts, waiting for connection...')
conn, addr = server.accept()

while True:
    print('Connected by', addr)
    data = conn.recv(1024)
    received = data.decode("utf-8").split(" ", 1)
    print(received)
    if received[0] == "SAVE":
        loop.run_until_complete(makeRequest('StopRecording'))
    elif received[0] == "RECORD":
        loop.run_until_complete(makeRequestWithParam('SetFilenameFormatting', {
                'filename-formatting': received[1],
        }))
        loop.run_until_complete(makeRequest('StartRecording'))
    else:
        break