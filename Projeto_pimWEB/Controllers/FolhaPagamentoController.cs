﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projeto_pimWEB.Data;
using Projeto_pimWEB.Filter;
using Projeto_pimWEB.Metodos;
using Projeto_pimWEB.Metodos.Calculos;
using Projeto_pimWEB.Models.Classes;

namespace Projeto_pimWEB.Controllers
{
	[UserFilterON]
	public class FolhaPagamentoController : Controller
	{

		private readonly IFuncionarioRepository _metodos;
		private readonly IFolhaPagamentoRepository _metodosFolha;

		public FolhaPagamentoController(IFuncionarioRepository metodo, IFolhaPagamentoRepository metodoFolha)
		{
			_metodos = metodo;
			_metodosFolha = metodoFolha;
		}


		public IActionResult CreatFolha(int id)
		{
			// Lista de beneficios
			// Lista de descontos

			Funcionario func = _metodos.GetFuncionario(id);
			func.dependentes = _metodos.GetAllDependentesFK(id);

			FolhaPagamento folha = new FolhaPagamento();
			folha.Funcionario = func;

			folha.Jornada = Calculos.CalcJornada(func.HoraSemanais);
			folha.SalarioBruto = Math.Round(Calculos.CalcSalarioBruto(func.Salario, folha.Beneficios, folha.Jornada), 2);
			folha.Inss = Math.Round(Calculos.CalcINSS(folha.SalarioBruto), 2);
			folha.Irrf = Math.Round(Calculos.CalcIRRF(folha.SalarioBruto, folha.Inss, func.dependentes), 2);
			folha.Fgts = Math.Round(Calculos.CalcFGTS(folha.SalarioBruto), 2);
			folha.SalarioLiquido = Math.Round(Calculos.CalcSalarioLiquido(folha.Inss, folha.Irrf, folha.SalarioBruto), 2);
			folha.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy");
			folha.MesAnoRef = DateTime.Now.ToString("MM/yyyy");

			return View(folha);
		}

		[HttpPost]
		public IActionResult CreatFolha(FolhaPagamento fp)
		{
			_metodosFolha.CreateFolha(fp);
			return RedirectToAction("Registro", "Funcionario");
		}


		public IActionResult CreateDesconto(int id)
		{
			return View();

		}

		public IActionResult CreateBenefios(int id)
		{
			return View();
		}

		public IActionResult RegistroFolha_Func(int id)
		{
			Funcionario func = _metodos.GetFuncionario(id);
			func.Folhas = _metodosFolha.GetAllFolhaPagamento_FK(id);

			return View(func);
		}

		public IActionResult Registro_Folhas()
		{
			List<FolhaPagamento> folha = _metodosFolha.GetAllFolhas();
			List<Funcionario> func = _metodos.GetAllFuncionarios();

			List<FolhaPagamento> ListaFinais = new List<FolhaPagamento>();

			foreach(FolhaPagamento fp in folha)
			{
				foreach(Funcionario funcionario in func)
				{
					if(fp.id_cod_func == funcionario.id_cod_func)
					{
						fp.Funcionario = funcionario;
						ListaFinais.Add(fp);
					}
				}
			}

			return View(ListaFinais);
		}
	}
}
