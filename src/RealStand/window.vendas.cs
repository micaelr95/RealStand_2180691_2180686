﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealStand
{
    partial class window
    {
        private bool novoCarroVenda = false;

        void CleanInputDetalhesVenda()
        {
            dateTimePickerDataVenda.Text = null;
            textBoxEstadoVendas.Text = "";
            maskedTextBoxValorVenda.Text = "";

            maskedTextBox1.Text = "";
            textBoxMarcaVendas.Text = "";
            textBoxModeloVendas.Text = "";
            comboBoxCombustivelVendas.SelectedItem = null; ;
            textBoxExtrasVendas.Text = "";
        }

        void IniciaFormVendas()
        {
            //Reseta a Ficha de Cliente
            labelNomeClienteSelecionadoVendas.Text = "Nenhum cliente selecionado";
            labelNifClienteSelecionadoVendas.Text = "*********";

            //Desseleciona todos os itens nas listboxes
            listBoxVendasDoCliente.SelectedIndex = -1;

            //Limpa todas as caixas de texto
            textBoxProcurarPorVendas.Text = "";
            comboBoxCampoVendas.SelectedItem = null;
            CleanInputDetalhesVenda();

            //Bloqueia groupboxes
            groupBoxVendasCliente.Enabled = false;
            groupBoxDetalhesVendaECarroVendas.Enabled = false;

        }

        private void listBoxClientesVendas_Click(object sender, EventArgs e)
        {
            Cliente selectedCliente = (Cliente)listBoxClientesVendas.SelectedItem;
            if (selectedCliente == null)
            {
                return;
            }

            listBoxVendasDoCliente.DataSource = selectedCliente.Venda.ToList();
            listBoxVendasDoCliente.SelectedIndex = -1;
            groupBoxVendasCliente.Enabled = true;
            buttonEditarVenda.Enabled = false;
            buttonAnularVenda.Enabled = false;

            //Preenche a ficha do cliente
            labelNomeClienteSelecionadoVendas.Text = selectedCliente.Nome;
            labelNifClienteSelecionadoVendas.Text = selectedCliente.NIF;
        }

        private void listBoxVendasDoCliente_Click(object sender, EventArgs e)
        {
            Venda selectedVenda = (Venda)listBoxVendasDoCliente.SelectedItem;

            //Insere as informações nas textboxes
            maskedTextBox1.Text = selectedVenda.CarroVenda.NumeroChassis;
            textBoxMarcaVendas.Text = selectedVenda.CarroVenda.Marca;
            textBoxModeloVendas.Text = selectedVenda.CarroVenda.Modelo;
            textBoxExtrasVendas.Text = selectedVenda.CarroVenda.Extras;
            dateTimePickerDataVenda.Value = selectedVenda.Data;
            textBoxEstadoVendas.Text = selectedVenda.Estado;
            maskedTextBoxValorVenda.Text = selectedVenda.Valor.ToString();
            switch (selectedVenda.CarroVenda.Combustivel)
            {
                case "Gasóleo":
                    comboBoxCombustivelVendas.SelectedIndex = 0;
                    break;
                case "Gasolina":
                    comboBoxCombustivelVendas.SelectedIndex = 1;
                    break;
                case "Diesel":
                    comboBoxCombustivelVendas.SelectedIndex = 2;
                    break;
                case "Gás":
                    comboBoxCombustivelVendas.SelectedIndex = 3;
                    break;
                case "Híbrido":
                    comboBoxCombustivelVendas.SelectedIndex = 4;
                    break;
            }

            //Reativa botões para alteração de dados
            buttonEditarVenda.Enabled = true;
            buttonAnularVenda.Enabled = true;
        }

        private void buttonGuardarVendas_Click(object sender, EventArgs e)
        {
            Cliente selectedCliente = (Cliente)listBoxClientesVendas.SelectedItem;
            string numeroChassis = maskedTextBox1.Text;
            string marca = textBoxMarcaVendas.Text;
            string modelo = textBoxModeloVendas.Text;
            string combustivel = comboBoxCombustivelVendas.Text;
            string extras = textBoxExtrasVendas.Text;
            DateTime dataVenda = dateTimePickerDataVenda.Value;
            string estado = textBoxEstadoVendas.Text;
            double valorVenda = double.Parse(maskedTextBoxValorVenda.Text.Replace('€', ' '));

            if (Carro.VerificaNumeroChassis(numeroChassis) && Carro.VerificaMarca(marca) && Carro.VerificaModelo(modelo))
            {
                if (novoCarroVenda)
                {
                    CarroVenda novoCarroVenda = new CarroVenda(extras, numeroChassis, marca, modelo, combustivel);
                    Venda novaVenda = new Venda(valorVenda, estado, dataVenda, novoCarroVenda);
                    try
                    {
                        standContainer.Carros.Add(novoCarroVenda);
                        selectedCliente.Venda.Add(novaVenda);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }
                else //Editar venda
                {
                    Venda selectedVenda = (Venda)listBoxVendasDoCliente.SelectedItem;
                    selectedVenda.CarroVenda.NumeroChassis = maskedTextBox1.Text;
                    selectedVenda.CarroVenda.Marca = textBoxMarcaVendas.Text;
                    selectedVenda.CarroVenda.Modelo = textBoxModeloVendas.Text;
                    selectedVenda.CarroVenda.Combustivel = comboBoxCombustivelVendas.Text;
                    selectedVenda.CarroVenda.Extras = textBoxExtrasVendas.Text;
                    selectedVenda.Data = dateTimePickerDataVenda.Value;
                    selectedVenda.Estado = textBoxEstadoVendas.Text;
                    selectedVenda.Valor = double.Parse(maskedTextBoxValorVenda.Text.Replace('€', ' '));
                }

                standContainer.SaveChanges();
                listBoxVendasDoCliente.DataSource = selectedCliente.Venda.ToList();
                listBoxVendasDoCliente.SelectedIndex = -1;
                CleanInputDetalhesVenda();
                groupBoxDetalhesVendaECarroVendas.Enabled = false;
                buttonAnularVenda.Enabled = false;
                buttonEditarVenda.Enabled = false;
            }
            else if (!Carro.VerificaNumeroChassis(numeroChassis))
            {
                MessageBox.Show("Número de Chassi incompleto. São 17 caracteres");
            }
            else if (!Carro.VerificaMarca(marca))
            {
                MessageBox.Show("Marca não inserida");
            }
            else if (!Carro.VerificaModelo(modelo))
            {
                MessageBox.Show("Modelo não inserido");
            }
            else if (!Carro.VerificaCombustivel(comboBoxCombustivelVendas.SelectedIndex))
            {
                MessageBox.Show("Combustível não selecionado");
            }
        }

        private void buttonCriarVenda_Click(object sender, EventArgs e)
        {
            novoCarroVenda = true;
            CleanInputDetalhesVenda();
            groupBoxDetalhesVendaECarroVendas.Enabled = true;
        }

        private void buttonEditarVenda_Click(object sender, EventArgs e)
        {
            novoCarroVenda = false;
            groupBoxDetalhesVendaECarroVendas.Enabled = true;
        }
        private void buttonAnularVenda_Click(object sender, EventArgs e)
        {
        }
    }
}