Imports System.Data
Imports System.Data.OleDb
Imports AjaxControlToolkit

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim strConexaoDados As String = ConfigurationManager.ConnectionStrings("strConexao").ConnectionString

    Public Editar As String = "Editar"

    Private Sub _Default_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblData.Text = FormatDateTime(Date.Now, DateFormat.LongDate)

            txtDtLimite.Attributes.Add("placeholder", "Data limite")
            txtDescricao.Attributes.Add("placeholder", "Descrição")
            txtObservacao.Attributes.Add("placeholder", "Observações")

            Call bindGrid()
        End If
    End Sub

    Protected Sub OnPageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvDados.PageIndex = e.NewPageIndex
        Call bindGrid()
    End Sub

    Sub bindGrid()
        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim dsDados As New DataSet()
        Dim strSQL As String = String.Empty

        Dim Expirado As String = String.Empty
        Dim Completo As String = String.Empty

        strSQL = "SELECT "
        strSQL += "tblTarefas.IDTarefa AS ID, tblTarefas.Completo, tblTarefas.DtLimite, tblTarefas.Descricao, tblTarefas.Observacao, "
        strSQL += "tblTipos.Descricao AS Tipo, "
        strSQL += "tblPrioridades.Descricao AS Prioridade "
        strSQL += "FROM "
        strSQL += "tblTarefas, tblTipos, tblPrioridades "
        strSQL += "WHERE "
        strSQL += "tblTarefas.IDTipo = tblTipos.IDTipo AND "
        strSQL += "tblTarefas.IDPrioridade = tblPrioridades.IDPrioridade "
        strSQL += "ORDER BY "
        strSQL += "tblPrioridades.Descricao"

        Try
            Using SQLConnection 'connection As New OleDbConnection(strConexaoDados)
                Dim command As New OleDbCommand(strSQL, SQLConnection)
                SQLConnection.Open()

                Dim reader As OleDbDataReader = command.ExecuteReader()

                If reader.HasRows Then
                    With gvDados
                        Dim dt As New DataTable()

                        dt.Columns.AddRange(New DataColumn() {New DataColumn("ID"), New DataColumn("Prioridade"), New DataColumn("Completo"), New DataColumn("DtLimite"),
                                        New DataColumn("Tipo"), New DataColumn("Descricao"), New DataColumn("Observacao"), New DataColumn("Expirado")})

                        While reader.Read
                            If reader("Completo") = True Then Completo = "SIM" Else Completo = "NÃO"
                            If reader("DtLimite") < Date.Now Then Expirado = "{EXPIRADO}" Else Expirado = ""

                            dt.Rows.Add(reader("ID"), reader("Prioridade"), Completo, Format(reader("DtLimite"), "dd/MM/yyyy"), reader("Tipo"), reader("Descricao"), reader("Observacao"), Expirado)
                        End While

                        .DataSource = dt
                        .DataBind()
                    End With
                End If

                reader.Close()
            End Using
        Catch ex As Exception
            Throw ex
        Finally
            SQLConnection.Close()
            SQLConnection.Dispose()
        End Try
    End Sub

    Sub montarComboPrioridades(Optional IDPrioridade As Integer = 0)
        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim dsDados As New DataSet()
        Dim strSQL As String = String.Empty

        strSQL = "SELECT "
        strSQL += "IDPrioridade AS ID, Descricao "
        strSQL += "FROM "
        strSQL += "tblPrioridades "
        strSQL += "ORDER BY "
        strSQL += "Descricao"

        Try
            Using SQLConnection
                Dim command As New OleDbCommand(strSQL, SQLConnection)
                SQLConnection.Open()

                Dim reader As OleDbDataReader = command.ExecuteReader()

                With cboPrioridades
                    .Items.Clear()

                    .Items.Add(New ListItem("Prioridade", 0))
                    While reader.Read
                        .Items.Add(New ListItem(reader("Descricao"), reader("ID")))
                    End While

                    If IDPrioridade <> 0 Then .SelectedValue = IDPrioridade
                End With

                reader.Close()
            End Using
        Catch ex As Exception
            Throw ex
        Finally
            SQLConnection.Close()
            SQLConnection.Dispose()
        End Try
    End Sub

    Sub montarComboTipos(Optional IDTipo As Integer = 0)
        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim dsDados As New DataSet()
        Dim strSQL As String = String.Empty

        strSQL = "SELECT "
        strSQL += "IDTipo AS ID, Descricao "
        strSQL += "FROM "
        strSQL += "tblTipos "
        strSQL += "ORDER BY "
        strSQL += "Descricao"

        Try
            Using SQLConnection
                Dim command As New OleDbCommand(strSQL, SQLConnection)
                SQLConnection.Open()

                Dim reader As OleDbDataReader = command.ExecuteReader()

                With cboTipos
                    .Items.Clear()

                    .Items.Add(New ListItem("Tipos", 0))
                    While reader.Read
                        .Items.Add(New ListItem(reader("Descricao"), reader("ID")))
                    End While

                    If IDTipo <> 0 Then .SelectedValue = IDTipo
                End With

                reader.Close()
            End Using
        Catch ex As Exception
            Throw ex
        Finally
            SQLConnection.Close()
            SQLConnection.Dispose()
        End Try
    End Sub

    Private Function salvarDados(ID As Integer, Acao As Integer) As Boolean '1-Insere, 2-Atualiza, 3-Deletar, 4-Atualizar Completo
        Dim retorno As Boolean = False

        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim strSQL As String

        Select Case Acao
            Case 1 'Insert
                strSQL = "INSERT INTO tblTarefas "
                strSQL += "(IDPrioridade, IDTipo, DtLimite, Descricao, Observacao) VALUES ("
                strSQL += cboPrioridades.SelectedItem.Value & ", "
                strSQL += cboTipos.SelectedItem.Value & ", "
                strSQL += "#" & txtDtLimite.Text & "#, "
                strSQL += "@Descricao, "
                strSQL += "@Observacao)"

                Using SQLConnection
                    Dim command As New OleDbCommand(strSQL, SQLConnection)

                    ' Use AddWithValue to assigns
                    command.Parameters.Add("@Descricao", OleDbType.VarChar).Value = txtDescricao.Text
                    command.Parameters.Add("@Observacao", OleDbType.VarChar).Value = txtObservacao.Text

                    Try
                        SQLConnection.Open()
                        command.ExecuteNonQuery()

                        retorno = True
                    Catch ex As Exception
                        'Throw
                        retorno = False
                    Finally
                        SQLConnection.Close()
                        SQLConnection.Dispose()
                    End Try
                End Using
            Case 2 'Update
                strSQL = "UPDATE tblTarefas SET "
                strSQL += "IDPrioridade = " & cboPrioridades.SelectedItem.Value & ", "
                strSQL += "IDTipo = " & cboTipos.SelectedItem.Value & ", "
                strSQL += "DtLimite = '" & txtDtLimite.Text & "', "
                strSQL += "Descricao = @Descricao, "
                strSQL += "Observacao = @Observacao "
                strSQL += "WHERE "
                strSQL += "IDTarefa = " & ID

                Using SQLConnection
                    Dim command As New OleDbCommand(strSQL, SQLConnection)

                    ' Use AddWithValue to assigns
                    command.Parameters.Add("@Descricao", OleDbType.VarChar).Value = txtDescricao.Text
                    command.Parameters.Add("@Observacao", OleDbType.VarChar).Value = txtObservacao.Text

                    Try
                        SQLConnection.Open()
                        command.ExecuteNonQuery()

                        retorno = True
                    Catch ex As Exception
                        'Throw
                        retorno = False
                    Finally
                        SQLConnection.Close()
                        SQLConnection.Dispose()
                    End Try
                End Using
            Case 3 'Delete
                strSQL = "DELETE FROM "
                strSQL += "tblTarefas "
                strSQL += "WHERE "
                strSQL += "IDTarefa = " & ID

                Using SQLConnection
                    Dim command As New OleDbCommand(strSQL, SQLConnection)

                    Try
                        SQLConnection.Open()
                        command.ExecuteNonQuery()

                        retorno = True
                    Catch ex As Exception
                        'Throw
                        retorno = False
                    Finally
                        SQLConnection.Close()
                        SQLConnection.Dispose()
                    End Try
                End Using
            Case 4
                strSQL = "UPDATE tblTarefas SET "
                strSQL += "Completo = True "
                strSQL += "WHERE "
                strSQL += "IDTarefa = " & lblID.Text

                Using SQLConnection
                    Dim command As New OleDbCommand(strSQL, SQLConnection)

                    Try
                        SQLConnection.Open()
                        command.ExecuteNonQuery()

                        retorno = True
                    Catch ex As Exception
                        'Throw
                        retorno = False
                    Finally
                        SQLConnection.Close()
                        SQLConnection.Dispose()
                    End Try
                End Using
        End Select

        Return retorno
    End Function

    Protected Sub Edit(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)

        lblID.Text = row.Cells(0).Text
        Call montarComboPrioridades(CInt(recuperarIDPrioridade(row.Cells(1).Text)))
        txtDtLimite.Text = row.Cells(3).Text
        Call montarComboTipos(CInt(recuperarIDTipo(row.Cells(4).Text)))
        txtDescricao.Text = row.Cells(5).Text
        txtObservacao.Text = row.Cells(6).Text

        btnExcluir.Visible = True
        btnCompleto.Enabled = True

        popup.Show()
    End Sub

    Protected Sub Save(ByVal sender As Object, ByVal e As EventArgs)
        If lblID.Text <> "0" Then
            If salvarDados(lblID.Text, 2) Then
                Call bindGrid()
            Else
                lblMensagem.Text = "Ocorreu um erro ao atualizar os dados !"
            End If
        ElseIf lblID.Text = "0" Then
            If salvarDados(lblID.Text, 1) Then
                Call bindGrid()
            Else
                lblMensagem.Text = "Ocorreu um erro ao inserir os dados !"
            End If
        End If
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs)
        lblID.Text = "0"
        txtDtLimite.Text = ""
        txtDescricao.Text = ""
        txtObservacao.Text = ""

        btnExcluir.Visible = False
        btnCompleto.Enabled = False

        Call montarComboPrioridades()
        Call montarComboTipos()

        popup.Show()
    End Sub

    Protected Sub Excluir(ByVal sender As Object, ByVal e As EventArgs)
        If salvarDados(lblID.Text, 3) Then
            Call bindGrid()
        Else
            lblMensagem.Text = "Ocorreu um erro ao excluir os dados !"
        End If
    End Sub

    Protected Sub Complete(ByVal sender As Object, ByVal e As EventArgs)
        If salvarDados(lblID.Text, 4) Then
            Call bindGrid()
        Else
            lblMensagem.Text = "Ocorreu um erro ao atualizar os dados !"
        End If
    End Sub

    Function recuperarIDPrioridade(Prioridade As String) As Integer
        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim strSQL As String = String.Empty

        Dim retorno As Integer = 0

        strSQL = "SELECT "
        strSQL += "IDPrioridade AS ID "
        strSQL += "FROM "
        strSQL += "tblPrioridades "
        strSQL += "WHERE "
        strSQL += "Descricao ='" & Prioridade & "'"

        Try
            Using SQLConnection
                Dim command As New OleDbCommand(strSQL, SQLConnection)
                SQLConnection.Open()

                Dim reader As OleDbDataReader = command.ExecuteReader()

                If reader.Read Then
                    retorno = reader("ID")
                End If

                reader.Close()
            End Using
        Catch ex As Exception
            Throw ex
        Finally
            SQLConnection.Close()
            SQLConnection.Dispose()
        End Try

        Return retorno
    End Function

    Function recuperarIDTipo(Tipo As String) As Integer
        Dim SQLConnection As New OleDbConnection(strConexaoDados)
        Dim strSQL As String = String.Empty

        Dim retorno As Integer = 0

        strSQL = "SELECT "
        strSQL += "IDTipo AS ID "
        strSQL += "FROM "
        strSQL += "tblTipos "
        strSQL += "WHERE "
        strSQL += "Descricao ='" & Tipo & "'"

        Try
            Using SQLConnection
                Dim command As New OleDbCommand(strSQL, SQLConnection)
                SQLConnection.Open()

                Dim reader As OleDbDataReader = command.ExecuteReader()

                If reader.Read Then
                    retorno = reader("ID")
                End If

                reader.Close()
            End Using
        Catch ex As Exception
            Throw ex
        Finally
            SQLConnection.Close()
            SQLConnection.Dispose()
        End Try

        Return retorno
    End Function
End Class
