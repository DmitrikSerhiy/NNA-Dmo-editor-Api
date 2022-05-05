﻿using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Repositories {
    // ReSharper disable once UnusedMember.Global
    internal sealed class EditorRepository : IEditorRepository {
        private readonly string _connectionString;

        #region sqlScripts

        private const string CreateDmoScript = 
            "INSERT INTO [dbo].[Dmos] ([Id],[DateOfCreation],[Name],[MovieTitle],[DmoStatus],[ShortComment],[NnaUserId],[HasBeats]) " +
            "VALUES(@id, @dateOfCreation, @name, @movieTitle, @dmoStatus, @shortComment, @nnaUserId, @hasBeats)";

        private const string LoadShortDmoScript =
            "SELECT Id, Name, MovieTitle, ShortComment, DmoStatus FROM Dmos WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string LoadDmoScript =
            "SELECT * FROM Dmos WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string UpdateShortDmoScript =
            "UPDATE Dmos SET Name = @name, MovieTitle = @movieTitle, ShortComment = @shortComment, DmoStatus = @dmoStatus WHERE Id = @id and NnaUserId = @nnaUserId";

        private const string UpdateBeatsJsonScript =
            "UPDATE Dmos set BeatsJson = @jsonBeats, HasBeats = @HasBeats where Id = @id and NnaUserId = @nnaUserId";

        private const string CreateBeatScript =
            "INSERT INTO [dbo].[Beats] ([Id],[DateOfCreation],[TempId],[BeatTime],[BeatTimeView],[Description],[Order],[UserId],[DmoId]) " +
            "VALUES(@id, @dateOfCreation, @tempId, @beatTime, @beatTimeView, @description, @order, @userId, @dmoId)";

        private const string DeleteBeatByIdScript =
            "DELETE FROM [dbo].[Beats] " +
            "WHERE Id = @id AND DmoId = @dmoId";
        
        private const string DeleteBeatByTempIdScript =
            "DELETE FROM [dbo].[Beats] " +
            "WHERE TempId = @tempId AND DmoId = @dmoId";

        private const string UpdateBeatByIdScript =
            "UPDATE [dbo].[Beats]" +
            "SET BeatTime = @beatTime, BeatTimeView = @beatTimeView, Description = @description " +
            "WHERE Id = @id AND UserId = @userId";
        
        private const string UpdateBeatByTempIdScript =
            "UPDATE [dbo].[Beats]" +
            "SET BeatTime = @beatTime, BeatTimeView = @beatTimeView, Description = @description " +
            "WHERE TempId = @tempId AND UserId = @userId";
        
        private const string ReorderBeatsOnAdd = 
            "UPDATE [dbo].[Beats] " +
            "SET [Order] = [Order] + 1 "+
            "WHERE [Order] >= @currenPosition AND Id != @currentBeatId";
        
        private const string ReorderBeatsOnDelete = 
            "UPDATE [dbo].[Beats] " +
            "SET [Order] = [Order] - 1  "+
            "WHERE [Order] >= @nextPosition";
        
        private const string LoadBeatForDeleteById = 
            "SELECT TOP (1) [Id], [Order], [DmoId] " +
            "FROM [dbo].[Beats] "+
            "WHERE Id = @id AND [DmoId] = @dmoId";
        
        private const string LoadBeatForDeleteByTempId = 
            "SELECT TOP (1) [Id], [Order], [DmoId] " +
            "FROM [dbo].[Beats] "+
            "WHERE TempId = @tempId AND [DmoId] = @dmoId";
        
        #endregion

        public EditorRepository(IConfiguration configuration) {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _connectionString = configuration.GetConnectionString("ConnectionForEditor") 
                                ?? throw new ApplicationException("Failed to establish db connection");
        }

        public async Task<bool> CreateDmoAsync(Dmo dmo) {
            var result = await ExecuteAsync(CreateDmoScript, new {
                id = dmo.Id,
                dateOfCreation = dmo.DateOfCreation,
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                dmoStatus = (short)DmoStatus.InProgress,
                shortComment = string.IsNullOrWhiteSpace(dmo.ShortComment) ? null : dmo.ShortComment,
                nnaUserId = dmo.NnaUserId,
                hasBeats = false
            });

            return result >= 1;
        }

        public async Task<Dmo> LoadShortDmoAsync(Guid id, Guid nnaUserId) {
            return await QueryAsync<Dmo>(LoadShortDmoScript, new { id, nnaUserId });
        }

        public async Task<Dmo> LoadDmoAsync(Guid id, Guid nnaUserId) {
            return await QueryAsync<Dmo>(LoadDmoScript, new { id, nnaUserId });
        }

        public async Task<bool> UpdateShortDmoAsync(Dmo dmo) {
            var result = await ExecuteAsync(UpdateShortDmoScript, new {
                name = dmo.Name,
                movieTitle = dmo.MovieTitle,
                shortComment = dmo.ShortComment,
                id = dmo.Id,
                nnaUserId = dmo.NnaUserId,
                dmoStatus = dmo.DmoStatus
            });
            
            return result >= 1;
        }

        public async Task<bool> UpdateJsonBeatsAsync(string jsonBeats, Guid id, Guid nnaUserId) {
            var result = await ExecuteAsync(UpdateBeatsJsonScript, new {
                jsonBeats, 
                id, 
                nnaUserId, 
                HasBeats = jsonBeats.Length > 0
            });
            return result >= 1;
        }

        public async Task<bool> InsertNewBeatAsync(Beat beat) {
            await ExecuteAsync(CreateBeatScript, new {
                id = beat.Id,
                dateOfCreation = beat.DateOfCreation,
                tempId = beat.TempId,
                beatTime = beat.BeatTime,
                beatTimeView = beat.BeatTimeView,
                description = beat.Description,
                order = beat.Order,
                userId = beat.UserId,
                dmoId = beat.DmoId
            });

            await ExecuteAsync(ReorderBeatsOnAdd, new {
                currenPosition = beat.Order,
                currentBeatId = beat.Id
            });
            
            return true;
        }

        public async Task<bool> UpdateBeatByIdAsync(Beat beat, Guid beatId) {
            var result = await ExecuteAsync(UpdateBeatByIdScript, new {
                beatTime = beat.BeatTime,
                beatTimeView = beat.BeatTimeView,
                description = beat.Description,
                id = beatId,
                userId = beat.UserId
            });

            return result >= 1;
        }

        public async Task<bool> UpdateBeatByTempIdAsync(Beat beat, string beatTempId) {
            var result = await ExecuteAsync(UpdateBeatByTempIdScript, new {
                beatTime = beat.BeatTime,
                beatTimeView = beat.BeatTimeView,
                description = beat.Description,
                tempId = beatTempId,
                userId = beat.UserId
            });

            return result >= 1;        
        }
        
        public async Task<bool> DeleteBeatByIdAsync(Beat beat) {
            await ExecuteAsync(DeleteBeatByIdScript, new {
                id = beat.Id,
                dmoId = beat.DmoId
            });

            await ExecuteAsync(ReorderBeatsOnDelete, new {
                nextPosition = beat.Order + 1
            });
            
            return true;
        }
        
        public async Task<bool> DeleteBeatByTempIdAsync(Beat beat) {
            await ExecuteAsync(DeleteBeatByTempIdScript, new {
                tempId = beat.TempId,
                dmoId = beat.DmoId
            });

            await ExecuteAsync(ReorderBeatsOnDelete, new {
                nextPosition = beat.Order + 1
            });
            
            return true;
        }

        public async Task<Beat> LoadBeatForDeleteByIdAsync(Guid id, Guid dmoId) {
            return await QueryAsync<Beat>(LoadBeatForDeleteById, new { id, dmoId });
        }
        
        public async Task<Beat> LoadBeatForDeleteByTempIdAsync(string tempId, Guid dmoId) {
            return await QueryAsync<Beat>(LoadBeatForDeleteByTempId, new { tempId, dmoId });
        }

        // Asynchronous Processing=true; <-- that's a part of connection string
        // todo: maybe I should migrate to ADO.NET for real async operations here 
        // because Dapper just does Task.FromResult under the hood. Need to investigate more deeply here
        // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-6.0
        // and inspect code of dapper more accurate
        

        private async Task<T> QueryAsync<T>(string request, object parameters) {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(request, parameters);
        }

        private async Task<int> ExecuteAsync(string command, object parameters) {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(command, parameters);
        }

        private void ExecuteTransactionAsync(List<(string, object)> commandsWithParameters) {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();

            try {
                foreach (var commandWithParameters in commandsWithParameters) {
                    connection.Execute(commandWithParameters.Item1, commandWithParameters.Item2);
                }

                transaction.Commit();
            }
            catch (Exception ex) {
                transaction.Rollback();
                throw;
            }

            //return Task.CompletedTask;
        }
    }
}
