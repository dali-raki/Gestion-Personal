﻿using GestionPersonnel.Models.TypeDePaiment;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.TypeDePaimentStorages
{
    public class TypeDePaiementStorage
    {
        private readonly string connectionString = ("Data Source=SQL6032.site4now.net;Initial Catalog=db_aa9d4f_gestionpersonnel;User Id=db_aa9d4f_gestionpersonnel_admin;Password=IAGE1234");

        private const string _selectAllQuery = "SELECT * FROM TypeDePaiement";
        private const string _selectByIdQuery = "SELECT * FROM TypeDePaiement WHERE TypePaiementID = @id";
        private const string _insertQuery = "INSERT INTO TypeDePaiement (NomTypePaiement) VALUES (@NomTypePaiement); SELECT SCOPE_IDENTITY();";
        private const string _updateQuery = "UPDATE TypeDePaiement SET NomTypePaiement = @NomTypePaiement WHERE TypePaiementID = @TypePaiementID;";
        private const string _deleteQuery = "DELETE FROM TypeDePaiement WHERE TypePaiementID = @TypePaiementID;";

        public TypeDePaiementStorage(IConfiguration configuration) =>
            connectionString = configuration.GetConnectionString("Data Source=SQL6032.site4now.net;Initial Catalog=db_aa9d4f_gestionpersonnel;User Id=db_aa9d4f_gestionpersonnel_admin;Password=IAGE1234");

        private static TypeDePaiement GetTypeDePaiementFromDataRow(DataRow row)
        {
            return new TypeDePaiement
            {
                TypePaiementID = (int)row["TypePaiementID"],
                NomTypePaiement = (string)row["NomTypePaiement"]
            };
        }

        public async Task<List<TypeDePaiement>> GetAll()
        {
            await using var connection = new SqlConnection(connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetTypeDePaiementFromDataRow(row)).ToList();
        }

        public async Task<TypeDePaiement?> GetById(int id)
        {
            await using var connection = new SqlConnection(connectionString);

            SqlCommand cmd = new(_selectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetTypeDePaiementFromDataRow(dataTable.Rows[0]);
        }

        public async Task Add(TypeDePaiement typeDePaiement)
        {
            await using var connection = new SqlConnection(connectionString);
            SqlCommand cmd = new(_insertQuery, connection);
            cmd.Parameters.AddWithValue("@NomTypePaiement", typeDePaiement.NomTypePaiement);

            connection.Open();
            var id = await cmd.ExecuteScalarAsync();
            typeDePaiement.TypePaiementID = Convert.ToInt32(id);
        }

        public async Task Update(TypeDePaiement typeDePaiement)
        {
            await using var connection = new SqlConnection(connectionString);
            SqlCommand cmd = new(_updateQuery, connection);
            cmd.Parameters.AddWithValue("@NomTypePaiement", typeDePaiement.NomTypePaiement);
            cmd.Parameters.AddWithValue("@TypePaiementID", typeDePaiement.TypePaiementID);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            await using var connection = new SqlConnection(connectionString);
            SqlCommand cmd = new(_deleteQuery, connection);
            cmd.Parameters.AddWithValue("@TypePaiementID", id);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
