using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Program().UpdateProductTyped();
        }

        public void SaveAccounts(AdventureStoreDataSet.DimAccountDataTable accounts)
        {
            AdventureStoreDataSetTableAdapters.DimAccountTableAdapter ata =
                new AdventureStoreDataSetTableAdapters.DimAccountTableAdapter();

            ata.Update(accounts);
        }


        public void InsertAccount(string description, string type)
        {
            ConnectionStringSettings css =
                ConfigurationManager.ConnectionStrings["AdventuresDB"];

            DbProviderFactory factory = DbProviderFactories.GetFactory(css.ProviderName);

            using(DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = css.ConnectionString;
                string sql = "insert into DimAccount (AccountDescription, AccountType)" +
                            " values (@desc, @type)";
                using(DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    var paramDesc = factory.CreateParameter();
                    paramDesc.ParameterName = "@desc";
                    paramDesc.Value = description;
                    command.Parameters.Add(paramDesc);

                    var paramType = factory.CreateParameter();
                    paramType.ParameterName = "@type";
                    paramType.Value = type;
                    command.Parameters.Add(paramType);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public DataRow GetAccount(string name)
        {
            ConnectionStringSettings css =
                ConfigurationManager.ConnectionStrings["AdventuresDB"];

            var dt = new DataTable();

            DbProviderFactory factory = DbProviderFactories.GetFactory(css.ProviderName);

            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = css.ConnectionString;
                var sql = "select * from DimAccount where AccountDescription = @name";
                using (var cmd = factory.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    var paramName = factory.CreateParameter();
                    paramName.ParameterName = "@name";
                    paramName.Value = name;
                    cmd.Parameters.Add(paramName);

                    using (var adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        connection.Open();
                        adapter.Fill(dt);
                        connection.Close();
                    }

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    else return null;
                }
            }
        }

        public void UpdateProductTyped()
        {
            AdventureStoreDataSet ds = new AdventureStoreDataSet();

            AdventureStoreDataSetTableAdapters.DimProductTableAdapter taProduct =
                new AdventureStoreDataSetTableAdapters.DimProductTableAdapter();
            AdventureStoreDataSetTableAdapters.DimProductSubcategoryTableAdapter taSubcategory =
                new AdventureStoreDataSetTableAdapters.DimProductSubcategoryTableAdapter();

            taProduct.Fill(ds.DimProduct);
            taSubcategory.Fill(ds.DimProductSubcategory);

            AdventureStoreDataSet.DimProductRow product = ds.DimProduct.NewDimProductRow();
            product.DimProductSubcategoryRow = ds.DimProductSubcategory[0];
            product.EnglishProductName = "Ball";
            product.SpanishProductName = "Pelota";
            product.FrenchProductName = "Balle";
            product.FinishedGoodsFlag = false;
            product.Color = "Brown";
            ds.DimProduct.AddDimProductRow(product);

            taProduct.Update(ds);
        }

        public void DeleteFinance(int id)
        {
            ConnectionStringSettings css =
                ConfigurationManager.ConnectionStrings["AdventuresDB"];

            DbProviderFactory factory = DbProviderFactories.GetFactory(css.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = css.ConnectionString;
                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "delete FactFinance where FinanceKey = @id";

                    DbParameter paramID = factory.CreateParameter();
                    paramID.ParameterName = "@id";
                    paramID.Value = id;

                    cmd.Parameters.Add(paramID);

                    conn.Open();
                    int count = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (count < 1)
                        throw new ArgumentOutOfRangeException("id", "Finance not found.");
                }
            }
        }

        public void UpdateProduct()
        {
            ConnectionStringSettings css =
                ConfigurationManager.ConnectionStrings["AdventuresDB"];

            DbProviderFactory factory = DbProviderFactories.GetFactory(css.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = css.ConnectionString;
                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "select ProductKey, SpanishProductName from DimProduct";
                    
                    using(DbDataAdapter adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        DbCommandBuilder builder = factory.CreateCommandBuilder();
                        builder.DataAdapter = adapter;

                        DataTable dt = new DataTable();
                        
                        adapter.Fill(dt);
                        adapter.FillSchema(dt, SchemaType.Mapped);

                        dt.Rows[0]["SpanishProductName"] = "Che pasa?";

                        adapter.Update(dt);
                    }
                }
            }
        }
    }
}
