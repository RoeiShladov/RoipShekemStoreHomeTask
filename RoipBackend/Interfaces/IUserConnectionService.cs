using Microsoft.AspNetCore.Mvc;
using RoipBackend.Models;

namespace RoipBackend.Interfaces
{
    public interface IUserConnectionService
    {
        void AddConnectionAsync(UserConnection connection);
        void RemoveConnectionAsync(UserConnection connection);

        //For future thought, if we want to store the connections in a database.
        //List<UserConnection> GetActiveConnections();
    }
}
