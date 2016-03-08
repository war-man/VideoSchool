﻿using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.Configuration;

namespace VideoSchool.Models
{
    public class DataBase
    {
        private MySqlConnection con;
        public Error error { get; private set; }
        public string query { get; private set; }

        public DataBase (Error error)
        {
            this.error = error;
            try
            {
                query = "OPEN CONNECTION TO MYSQL";
                con = new MySqlConnection(
                    WebConfigurationManager.ConnectionStrings["conn"].ConnectionString);
                con.Open();
            }
            catch (Exception ex)
            {
                error.MarkDBaseError(ex, "Error connection to mysql", query);
                con = null;
                throw ex;
            }
        }

        ~DataBase ()
        {
            try
            {
                con.Close();
            }
            catch 
            {

            }
        }

        public DataTable Select (string myquery)
        {
            if (error.AnyError()) return null;
            try
            {
                query = myquery;
                DataTable table = new DataTable();
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
                return table;
            }
            catch (Exception ex)
            {
                error.MarkDBaseError(ex, "Error on Select query", query);
                throw ex;
            }
        }

        public string Scalar (string myquery)
        {
            if (error.AnyError()) return null;
            try
            {
                query = myquery;
                DataTable table = new DataTable();
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
                if (table.Rows.Count == 0)
                    return "";
                return table.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                error.MarkDBaseError(ex, "Error on Scalar query", query);
                throw ex;
            }
        }

        public long Insert(string myquery)
        {
            if (error.AnyError()) return -1;
            try
            {
                query = myquery;
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                error.MarkDBaseError(ex, "Error on Insert query", query);
                throw ex;
            }
        }

        public int Update(string myquery)
        {
            if (error.AnyError()) return -1;
            try
            {
                query = myquery;
                MySqlCommand cmd = new MySqlCommand(query, con);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                error.MarkDBaseError(ex, "Error on Update query", query);
                throw ex;
            }
        }

        public string addslashes (string text)
        {
            if (text == null) return "";
            return text.Replace ("\'", "\\\'");
        }

    }
}