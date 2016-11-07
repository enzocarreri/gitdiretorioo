using Projeto1.Models;
using Projeto1.Models.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Projeto1.Controllers
{
    public class PedidoController : Controller
    {
        public ActionResult RealizarPedido()
        {
            string selecionado = Request.QueryString["item"];
            string cliente = Request.QueryString["cliente"];

            string pedido=null;
           
            pedido =selecionado.Replace(",", "");

            string empresa = pedido.Substring(0, 2);
            PedidoDAO dao = new PedidoDAO();

            List<ModProduto> ped = dao.ListarPedido(pedido);

            ViewBag.empresa = empresa;
            ViewBag.cliente = cliente;

            return View(ped);

        }
        public ActionResult FinalizarPedido()
        {
            string selecionado = Request.QueryString["item"];
            string cliente = Request.QueryString["cliente"];
            string pedido = null;

            pedido = selecionado.Replace(",", "");

            
            PedidoDAO dao = new PedidoDAO();

            string ao = dao.FinalizarPedido(pedido,cliente);


            ViewBag.nome = selecionado;
            ViewBag.nome2 = ao;
            ViewBag.cliente = cliente;
            return View();

        }

    }
}