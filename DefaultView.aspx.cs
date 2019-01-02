using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.OleDb;
using AjaxControlToolkit;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

partial class DefaultView : System.Web.UI.Page
{
    string strConexaoDados = ConfigurationManager.ConnectionStrings["strConexao"].ConnectionString;//.ConnectionStrings("strConexao").ConnectionString;

    public string Editar = "Editar";

    private void _Default_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            lblData.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

            txtDtLimite.Attributes.Add("placeholder", "Data limite");
            txtDescricao.Attributes.Add("placeholder", "Descrição");
            txtObservacao.Attributes.Add("placeholder", "Observações");

            bindGrid();
        }
    }

    protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDados.PageIndex = e.NewPageIndex;
        bindGrid();
    }

    public void bindGrid()
    {
        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        DataSet dsDados = new DataSet();
        string strSQL = string.Empty;

        string Expirado = string.Empty;
        string Completo = string.Empty;

        strSQL = "SELECT ";
        strSQL += "tblTarefas.IDTarefa AS ID, tblTarefas.Completo, tblTarefas.DtLimite, tblTarefas.Descricao, tblTarefas.Observacao, ";
        strSQL += "tblTipos.Descricao AS Tipo, ";
        strSQL += "tblPrioridades.Descricao AS Prioridade ";
        strSQL += "FROM ";
        strSQL += "tblTarefas, tblTipos, tblPrioridades ";
        strSQL += "WHERE ";
        strSQL += "tblTarefas.IDTipo = tblTipos.IDTipo AND ";
        strSQL += "tblTarefas.IDPrioridade = tblPrioridades.IDPrioridade ";
        strSQL += "ORDER BY ";
        strSQL += "tblPrioridades.Descricao";

        try
        {
            using (SQLConnection) // connection As New OleDbConnection(strConexaoDados)
            {
                OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);
                SQLConnection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    {
                        var withBlock = gvDados;
                        DataTable dt = new DataTable();

                        dt.Columns.AddRange(new DataColumn[] { new DataColumn("ID"), new DataColumn("Prioridade"), new DataColumn("Completo"), new DataColumn("DtLimite"), new DataColumn("Tipo"), new DataColumn("Descricao"), new DataColumn("Observacao"), new DataColumn("Expirado") });

                        while (reader.Read())
                        {
                            if (reader["Completo"].Equals(true))
                                Completo = "SIM";
                            else
                                Completo = "NÃO";
                            if (DateTime.Parse(reader["DtLimite"].ToString()) < DateTime.Now)
                                Expirado = "{EXPIRADO}";
                            else
                                Expirado = "";

                            dt.Rows.Add(reader["ID"], reader["Prioridade"], Completo, String.Format(reader["DtLimite"].ToString(), "dd/MM/yyyy"), reader["Tipo"], reader["Descricao"], reader["Observacao"], Expirado);
                        }

                        withBlock.DataSource = dt;
                        withBlock.DataBind();
                    }
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }
    }

    public void montarComboPrioridades(int IDPrioridade = 0)
    {
        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        DataSet dsDados = new DataSet();
        string strSQL = string.Empty;

        strSQL = "SELECT ";
        strSQL += "IDPrioridade AS ID, Descricao ";
        strSQL += "FROM ";
        strSQL += "tblPrioridades ";
        strSQL += "ORDER BY ";
        strSQL += "Descricao";

        try
        {
            using (SQLConnection)
            {
                OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);
                SQLConnection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                {
                    var withBlock = cboPrioridades;
                    int zero = 0;
                    withBlock.Items.Clear();

                    withBlock.Items.Add(new ListItem("Prioridade", zero.ToString()));
                    while (reader.Read())
                        withBlock.Items.Add(new ListItem(reader["Descricao"].ToString(), reader["ID"].ToString()));

                    if (IDPrioridade != 0)
                        withBlock.SelectedValue = IDPrioridade.ToString();
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }
    }

    public void montarComboTipos(int IDTipo = 0)
    {
        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        DataSet dsDados = new DataSet();
        string strSQL = string.Empty;

        strSQL = "SELECT ";
        strSQL += "IDTipo AS ID, Descricao ";
        strSQL += "FROM ";
        strSQL += "tblTipos ";
        strSQL += "ORDER BY ";
        strSQL += "Descricao";

        try
        {
            using (SQLConnection)
            {
                OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);
                SQLConnection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                {
                    var withBlock = cboTipos;
                    withBlock.Items.Clear();

                    withBlock.Items.Add(new ListItem("Tipos", Convert.ToString(0)));
                    while (reader.Read())
                        withBlock.Items.Add(new ListItem(reader["Descricao"].ToString(), reader["ID"].ToString()));

                    if (IDTipo != 0)
                        withBlock.SelectedValue = IDTipo.ToString();
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }
    }

    private bool salvarDados(int ID, int Acao) // 1-Insere, 2-Atualiza, 3-Deletar, 4-Atualizar Completo
    {
        bool retorno = false;

        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        string strSQL;

        switch (Acao)
        {
            case 1 // Insert
           :
                {
                    strSQL = "INSERT INTO tblTarefas ";
                    strSQL += "(IDPrioridade, IDTipo, DtLimite, Descricao, Observacao) VALUES (";
                    strSQL += cboPrioridades.SelectedItem.Value + ", ";
                    strSQL += cboTipos.SelectedItem.Value + ", ";
                    strSQL += "#" + txtDtLimite.Text + "#, ";
                    strSQL += "@Descricao, ";
                    strSQL += "@Observacao)";

                    using (SQLConnection)
                    {
                        OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);

                        // Use AddWithValue to assigns
                        command.Parameters.Add("@Descricao", OleDbType.VarChar).Value = txtDescricao.Text;
                        command.Parameters.Add("@Observacao", OleDbType.VarChar).Value = txtObservacao.Text;

                        try
                        {
                            SQLConnection.Open();
                            command.ExecuteNonQuery();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            // Throw
                            retorno = false;
                        }
                        finally
                        {
                            SQLConnection.Close();
                            SQLConnection.Dispose();
                        }
                    }

                    break;
                }

            case 2 // Update
     :
                {
                    strSQL = "UPDATE tblTarefas SET ";
                    strSQL += "IDPrioridade = " + cboPrioridades.SelectedItem.Value + ", ";
                    strSQL += "IDTipo = " + cboTipos.SelectedItem.Value + ", ";
                    strSQL += "DtLimite = '" + txtDtLimite.Text + "', ";
                    strSQL += "Descricao = @Descricao, ";
                    strSQL += "Observacao = @Observacao ";
                    strSQL += "WHERE ";
                    strSQL += "IDTarefa = " + ID;

                    using (SQLConnection)
                    {
                        OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);

                        // Use AddWithValue to assigns
                        command.Parameters.Add("@Descricao", OleDbType.VarChar).Value = txtDescricao.Text;
                        command.Parameters.Add("@Observacao", OleDbType.VarChar).Value = txtObservacao.Text;

                        try
                        {
                            SQLConnection.Open();
                            command.ExecuteNonQuery();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            // Throw
                            retorno = false;
                        }
                        finally
                        {
                            SQLConnection.Close();
                            SQLConnection.Dispose();
                        }
                    }

                    break;
                }

            case 3 // Delete
     :
                {
                    strSQL = "DELETE FROM ";
                    strSQL += "tblTarefas ";
                    strSQL += "WHERE ";
                    strSQL += "IDTarefa = " + ID;

                    using (SQLConnection)
                    {
                        OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);

                        try
                        {
                            SQLConnection.Open();
                            command.ExecuteNonQuery();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            // Throw
                            retorno = false;
                        }
                        finally
                        {
                            SQLConnection.Close();
                            SQLConnection.Dispose();
                        }
                    }

                    break;
                }

            case 4:
                {
                    strSQL = "UPDATE tblTarefas SET ";
                    strSQL += "Completo = True ";
                    strSQL += "WHERE ";
                    strSQL += "IDTarefa = " + lblID.Text;

                    using (SQLConnection)
                    {
                        OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);

                        try
                        {
                            SQLConnection.Open();
                            command.ExecuteNonQuery();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            // Throw
                            retorno = false;
                        }
                        finally
                        {
                            SQLConnection.Close();
                            SQLConnection.Dispose();
                        }
                    }

                    break;
                }
        }

        return retorno;
    }

    protected void Edit(object sender, EventArgs e)
    {
        GridViewRow row = ((GridViewRow)(((LinkButton)(sender)).Parent.Parent));

        lblID.Text = row.Cells[0].Text;
        montarComboPrioridades(System.Convert.ToInt32(recuperarIDPrioridade(row.Cells[1].Text)));
        txtDtLimite.Text = row.Cells[3].Text;
        montarComboTipos(System.Convert.ToInt32(recuperarIDTipo(row.Cells[4].Text)));
        txtDescricao.Text = row.Cells[5].Text;
        txtObservacao.Text = row.Cells[6].Text;

        btnExcluir.Visible = true;
        btnCompleto.Enabled = true;

        popup.Show();
    }

    protected void Save(object sender, EventArgs e)
    {
        if (lblID.Text != "0")
        {
            if (salvarDados(Int32.Parse(lblID.Text), 2))
                bindGrid();
            else
                lblMensagem.Text = "Ocorreu um erro ao atualizar os dados !";
        }
        else if (lblID.Text == "0")
        {
            if (salvarDados(Int32.Parse(lblID.Text), 1))
                bindGrid();
            else
                lblMensagem.Text = "Ocorreu um erro ao inserir os dados !";
        }
    }

    protected void Add(object sender, EventArgs e)
    {
        lblID.Text = "0";
        txtDtLimite.Text = "";
        txtDescricao.Text = "";
        txtObservacao.Text = "";

        btnExcluir.Visible = false;
        btnCompleto.Enabled = false;

        montarComboPrioridades();
        montarComboTipos();

        popup.Show();
    }

    protected void Excluir(object sender, EventArgs e)
    {
        if (salvarDados(Int32.Parse(lblID.Text), 3)) {
            bindGrid();
        }
            
        else
        {
            lblMensagem.Text = "Ocorreu um erro ao excluir os dados !";
        }
            
    }

    protected void Complete(object sender, EventArgs e)
    {
        if (salvarDados(Int32.Parse(lblID.Text), 4))
            bindGrid();
        else
            lblMensagem.Text = "Ocorreu um erro ao atualizar os dados !";
    }

    public int recuperarIDPrioridade(string Prioridade)
    {
        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        string strSQL = string.Empty;

        int retorno = 0;

        strSQL = "SELECT ";
        strSQL += "IDPrioridade AS ID ";
        strSQL += "FROM ";
        strSQL += "tblPrioridades ";
        strSQL += "WHERE ";
        strSQL += "Descricao ='" + Prioridade + "'";

        try
        {
            using (SQLConnection)
            {
                OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);
                SQLConnection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                    retorno = int.Parse(reader["ID"].ToString()); // (reader["ID"]

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }

        return retorno;
    }

    public int recuperarIDTipo(string Tipo)
    {
        OleDbConnection SQLConnection = new OleDbConnection(strConexaoDados);
        string strSQL = string.Empty;

        int retorno = 0;

        strSQL = "SELECT ";
        strSQL += "IDTipo AS ID ";
        strSQL += "FROM ";
        strSQL += "tblTipos ";
        strSQL += "WHERE ";
        strSQL += "Descricao ='" + Tipo + "'";

        try
        {
            using (SQLConnection)
            {
                OleDbCommand command = new OleDbCommand(strSQL, SQLConnection);
                SQLConnection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                    retorno = int.Parse(reader["ID"].ToString());

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }

        return retorno;
    }
}
