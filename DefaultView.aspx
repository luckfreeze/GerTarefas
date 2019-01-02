<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DefaultView.aspx.cs" Inherits="DefaultView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gerenciador de Tarefas</title>

    <!-- CSS Geral -->
    <link href="css/bootstrap.css" rel="stylesheet" />
    <link href="css/modal.css" rel="stylesheet" />
    <!-- Para icones do grid -->
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.2.0/css/all.css" integrity="sha384-hWVjflwFxL6sNzntih27bfxkr27PmbbK/iSvJ+a4+0owXq79v+lsFkW54bOGbiDQ" crossorigin="anonymous" />

    <!-- Para modal -->
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="js/jquery.blockUI.js" type="text/javascript"></script>
    <script type = "text/javascript">
        function BlockUI(elementID) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function() {
                $("#" + elementID).block({ message: '<table align = "center"><tr><td>' +
         '<img src="images/loadingAnim.gif"/></td></tr></table>',
                    css: {},
                    overlayCSS: { backgroundColor: '#000000', opacity: 0.6
                    }
                });
            });
            prm.add_endRequest(function() {
                $("#" + elementID).unblock();
            });
        }
        $(document).ready(function() {

            BlockUI("<%=pnlAddEdit.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        function Hidepopup() {
            $find("popup").hide();
            return false;
        }
    </script> 
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="container">
                    <div class="row">
                        <div class="col-12 text-left">&nbsp;</div>
                        <div class="col-12 text-left">&nbsp;</div>
                    </div>

                    <!-- Botão e Data/Hora -->
                    <div class="row">
                        <div class="col-2 text-left"><asp:Button CssClass="btn btn-outline-primary" ID="btnNovaTarefa" runat="server" Text="Adicionar nova tarefa" OnClick="Add" /></div>
                        <div class="col-5"></div>
                        <div class="col-5 text-right"><h4><asp:Label ID="lblData" runat="server" /></h4></div>
                    </div>

                    <div class="row">
                        <div class="col-12 text-left">&nbsp;</div>
                    </div>

                    <!-- GRID -->
                    <div class="row">
                        <div class="col-12">
                            <asp:GridView CssClass="table table-hover" OnPageIndexChanging="OnPageIndexChanging" Width="100%" ID="gvDados" AutoGenerateColumns="False" runat="server" AllowPaging="true" DataKeyNames="ID"
                                PageSize="10" PagerSettings-Position="Bottom" PagerStyle-HorizontalAlign="Center" CellPadding="8" CellSpacing="1" HorizontalAlign="Center" GridLines="None" ShowFooter="false" HeaderStyle-CssClass="thead-dark" HeaderStyle-HorizontalAlign="Center">
                                <PagerStyle CssClass="pagination" BackColor="White" />
                                <Columns>
                                    <asp:BoundField DataField="ID" ItemStyle-Width="1%" ItemStyle-ForeColor="White" />
                                    <asp:BoundField DataField="Prioridade" HeaderText="Prioridade" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%" />
                                    <asp:BoundField DataField="Completo" HeaderText="Completo" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="5%" />
                                    <asp:BoundField DataField="DtLimite" HeaderText="Dt.Limite" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="Tipo" HeaderText="Tipo" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" />
                                    <asp:BoundField DataField="Descricao" HeaderText="Descricao" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25%" />
                                    <asp:BoundField DataField="Observacao" HeaderText="Obs." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="30%" />
                                    <asp:BoundField DataField="Expirado" HeaderText="Expirado" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-ForeColor="Red" />
                                    <asp:TemplateField ItemStyle-Width="5%" HeaderText="">
                                        <ItemTemplate>
                                            <asp:LinkButton CssClass="far fa-edit fa-1x" ID="lnkEdit" runat="server" OnClick="Edit" ToolTip="Editar" />
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                </Columns>                                    
                            </asp:GridView>
                        </div>
                    </div>

                    <asp:Panel ID="pnlAddEdit" runat="server" CssClass="bg-transparent" style = "display:none">
                        <div class="modal-content">
                            <!--Header -->
                            <div class="modal-header" style="background-color:#d3d2d2;">
                                <h5 class="modal-title" id="EditModalLabel">Tarefa</h5>
                                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="return Hidepopup()" ToolTip="Fechar">
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </asp:LinkButton>
                                
                            </div>
                            <!-- /Header -->
                            <!-- Main -->
                              <div class="modal-body">
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-12 text-center"><asp:Label ID="lblMensagem" runat="server" ForeColor="Red" /></div>
                                    </div>
                                    <asp:Label ID="lblID" runat="server" Visible="false" Text="0" />
                                    <div class="row">
                                        <div class="col-4"><asp:DropDownList CssClass="form-control" ID="cboPrioridades" runat="server" /></div>
                                        <div class="col-4"><asp:TextBox CssClass="form-control" ID="txtDtLimite" MaxLength="10" runat="server" /></div>
                                        <div class="col-4"><asp:DropDownList CssClass="form-control" ID="cboTipos" runat="server" /></div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-12"><asp:TextBox CssClass="form-control" ID="txtDescricao" MaxLength="30" runat="server" /></div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-12"><asp:TextBox CssClass="form-control" ID="txtObservacao" MaxLength="50" runat="server" /></div>
                                    </div>
                                </div>
                            </div>
                            <!-- /Main -->
                            <!-- Footer -->
                            <div class="modal-footer">
                                <div class="col-3">&nbsp;</div>
                                <div class="col-3 text-center">
                                    <asp:Button CssClass="btn btn-outline-primary" ID="btnSalvar" runat="server" Text="&nbsp;&nbsp;&nbsp;Salvar&nbsp;&nbsp;&nbsp;" OnClick = "Save" /></div>
                                <div class="col-3 text-center"><asp:Button CssClass="btn btn-outline-secondary" ID="btnCompleto" runat="server" Text="Completo" Enabled="false" OnClick="Complete"/></div>
                                <div class="col-3 text-right"><asp:LinkButton CssClass="far fa-trash-alt fa-2x text-black-50" ID="btnExcluir" runat="server" OnClick="Excluir" ToolTip="Excluir" /></div>
                            </div>
                            <!-- /Footer -->
                        </div>
                    </asp:Panel>

                    <asp:LinkButton ID="lnkFake" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popup" runat="server" DropShadow="false" PopupControlID="pnlAddEdit" TargetControlID = "lnkFake" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
                </div>
            </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID = "gvDados" />
            <asp:AsyncPostBackTrigger ControlID = "btnSalvar" />
            <asp:AsyncPostBackTrigger ControlID = "btnExcluir" />
            <asp:AsyncPostBackTrigger ControlID = "btnNovaTarefa" />
            <asp:AsyncPostBackTrigger ControlID = "btnCompleto" />
        </Triggers>
    </asp:UpdatePanel>
    <!-- /Modal -->
    </form>
</body>
</html>
