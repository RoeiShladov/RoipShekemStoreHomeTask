const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:5001/statusHub')
  .build();

connection.on('ReceiveStatus', (userId: string, isConnected: boolean) => {
  console.log(`User ${userId} is ${isConnected ? 'connected' : 'disconnected'}`);
});

connection.start().catch(err => console.error(err));
