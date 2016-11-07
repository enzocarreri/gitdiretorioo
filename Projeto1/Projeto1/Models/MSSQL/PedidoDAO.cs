using Aprendendo.Models.MSSQL;
using MySql.Data.MySqlClient;
using Projeto1.Models.Validacoes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Projeto1.Models.MSSQL
{
    public class PedidoDAO : Erros
    {
        internal List<ModProduto> ListarPedido(string pedido)
        {
            // Instancia nossos objetos
            List<ModProduto> oferece = new List<ModProduto>();

            // Instancia nossa Conexao
            Conexao conexao = new Conexao(TipoConexao.Conexao.WebConfig);

            // Se existe erro na conexao move o erro para a classe de acesso 
            if (conexao.ExisteErro())
            {
                setMensagemErro(conexao.mErro);
                return oferece;
            }

            try
            {
                MySqlDataReader reader;
                String query = "";
                
                int i = 2;

                while (i < pedido.Length)
                {
                    if (i == 2)
                    {
                        query =  " p.codigoproduto ='" + pedido.Substring(i, 2) + "'";
                        i = i + 2;
                    }
                    else
                    {
                        query = query + " or p.codigoproduto='" + pedido.Substring(i, 2) + "'";
                        i = i + 2;
                    }
                    
                }
                
                             
                
                query = "select p.codigoproduto, p.nomeproduto, t.descricao, t.codigotipo from produto p " +
                    "INNER JOIN tipoproduto t ON p.codtipoproduto = t.codigotipo " +
                    "WHERE " + query + " ORDER BY t.descricao";





                





                MySqlCommand cmd = new MySqlCommand(query, conexao.conn);

                // define que o comando é um texto
                cmd.CommandType = System.Data.CommandType.Text;

                // Abre nossa Conexao
                if (conexao.OpenConexao() == false)
                {

                    setMensagemErro(conexao.mErro);
                    return oferece;
                }

                reader = cmd.ExecuteReader();

                // recebe os dados de nossa consulta
                while (reader.Read())
                {

                    oferece.Add(read_Produto(reader));
                }
            }
            catch (SqlException e)
            {
                // Trata os erros de nossa conexão
                setMensagemErro(e.Message.ToString());
            }

            // Fecha nossa Conexao
            conexao.CloseConexao();

            // Retorna nossa lista de dados
            return oferece;
        }

        internal string FinalizarPedido(string pedido, string cliente)
        {
            // Instancia nossos objetos
            List<ModPedido> itensPedido = new List<ModPedido>();

            // Instancia nossa Conexao
            Conexao conexao = new Conexao(TipoConexao.Conexao.WebConfig);

            // Se existe erro na conexao move o erro para a classe de acesso 
            if (conexao.ExisteErro())
            {
                setMensagemErro(conexao.mErro);
                return "erro";
            }

            try
            {
                MySqlDataReader reader;
                string query1 = "",query2="",query3="";
                string empresa = "";
               
               
                DateTime saveNow = DateTime.Now;
                
                empresa = pedido.Substring(0, 2);
                query1 = "insert into pedido(codclientepedido,codempresapedido,datavenda) values (  '" + cliente + "', '" + empresa + "', now())";
                

                query2 = "select codigopedido from pedido order by codigopedido desc limit 1";
                

                

               



                // Abre nossa Conexao
                if (conexao.OpenConexao() == false)
                {

                    setMensagemErro(conexao.mErro);
                    return "erro";
                }
                MySqlCommand cmd1 = new MySqlCommand(query1, conexao.conn);
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();

                MySqlCommand cmd2 = new MySqlCommand(query2, conexao.conn);
                cmd2.CommandType = System.Data.CommandType.Text;
                reader = cmd2.ExecuteReader();

                string codigo = "";
                while (reader.Read())
                {

                    itensPedido.Add(read_Pedido(reader));
                    codigo = reader["codigopedido"].ToString();
                }
                int i = 2;
                reader.Dispose();
                while (i < pedido.Length)
                {
                    query3 = "INSERT INTO itenspedido (codprodutopedido,coditenspedido) values ( '" + Convert.ToInt32(pedido.Substring(i, 2)) + "', '" + Convert.ToInt32(codigo) + "' )";
                    i = i + 2;
                    MySqlCommand cmd3 = new MySqlCommand(query3, conexao.conn);
                    try
                    {
                        cmd3.ExecuteNonQuery();
                        
                        
                    }
                    catch (SystemException ex)
                    {
                        return "falha";
                    }
                    finally
                    {
                        cmd2.Dispose();
                    }
                }
               
               
                ;





            }
            catch (SqlException e)
            {
                // Trata os erros de nossa conexão
                setMensagemErro(e.Message.ToString());
            }
           
            // Fecha nossa Conexao
            conexao.CloseConexao();

            return "Pedido realizado";
        }

        private ModPedido read_Pedido(MySqlDataReader reader)
        {
            ModPedido pedido = new ModPedido();
            
            pedido.codigoPedido = ConvertReader.ConverteInt(reader["codigopedido"]);
            
           

            

            return pedido;
        }

        private ModProduto read_Produto(MySqlDataReader reader)
        {
            ModProduto produto = new ModProduto();
            produto.codigoProduto = ConvertReader.ConverteInt(reader["codigoproduto"]);
            produto.nomeProduto = (string)reader["nomeproduto"] ?? "";

            produto.tipoProduto = read_TipoProduto(reader);

            return produto;
        }
        private ModTipoProduto read_TipoProduto(MySqlDataReader reader)
        {
            ModTipoProduto tipo = new ModTipoProduto();
            tipo.codigoTipo = ConvertReader.ConverteInt(reader["codigoTipo"]);
            tipo.descricao = (string)reader["descricao"] ?? "";


            return tipo;
        }
    }
}