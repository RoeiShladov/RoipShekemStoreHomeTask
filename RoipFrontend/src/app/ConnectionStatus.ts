// Establish a connection to the SignalR hub
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/userConnectionHub") // Match the hub route in your server
  .withAutomaticReconnect() // Automatically reconnect if the connection is lost
  .build();

// Function to update the UI with the list of connected users
function updateConnectedUsers(connectedUsers) {
  const userListElement = document.getElementById("connectedUsersList");
  userListElement.innerHTML = ""; // Clear the current list

  connectedUsers.forEach(user => {
    const listItem = document.createElement("li");
    listItem.textContent = `${user.key}: ${user.value}`; // Display user info
    userListElement.appendChild(listItem);
  });
}

// Start the SignalR connection
async function startConnection() {
  try {
    await connection.start();
    console.log("SignalR connection established.");
  } catch (err) {
    console.error("Error establishing SignalR connection:", err);
    setTimeout(startConnection, 5000); // Retry connection after 5 seconds
  }
}

// Listen for updates from the hub
connection.on("UpdateConnectedUsers", (connectedUsers) => {
  updateConnectedUsers(connectedUsers);
});

// Start the connection
startConnection();
