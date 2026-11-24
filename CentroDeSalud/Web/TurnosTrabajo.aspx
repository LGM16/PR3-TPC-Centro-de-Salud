<%@ Page Title="Turnos de trabajo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TurnosTrabajo.aspx.cs" Inherits="Web.TurnosTrabajo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:HiddenField ID="hfId" runat="server" Value="0" />
        <asp:Label ID="lblFormulario" runat="server" CssClass="form-label fw-bold d-block" Text="Nuevo turno de trabajo"></asp:Label>
        <div class="row g-2 align-items-start mb-4">
            <div class="col-md-4">
                <asp:Label runat="server" AssociatedControlID="txtDescripcion" CssClass="form-label" Text="Descripción"></asp:Label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server" ControlToValidate="txtDescripcion"
                    ErrorMessage="La descripción es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" AssociatedControlID="txtEntrada" CssClass="form-label" Text="Entrada"></asp:Label>
                <asp:TextBox ID="txtEntrada" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEntrada" runat="server" ControlToValidate="txtEntrada"
                    ErrorMessage="La entrada es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" AssociatedControlID="txtSalida" CssClass="form-label" Text="Salida"></asp:Label>
                <asp:TextBox ID="txtSalida" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSalida" runat="server" ControlToValidate="txtSalida"
                    ErrorMessage="La salida es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4 pt-4">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnCancelar_Click" />
            </div>
        </div>

        <asp:Panel runat="server" DefaultButton="btnFiltrar" CssClass="row g-2 mb-3">
            <div class="col-md-6">
                <asp:TextBox ID="txtFiltro" runat="server" CssClass="form-control" placeholder="Buscar por descripción..."></asp:TextBox>
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-primary" CausesValidation="false" OnClick="btnFiltrar_Click" />
                <asp:Button ID="btnLimpiarFiltro" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="btnLimpiarFiltro_Click" />
            </div>
        </asp:Panel>

        <asp:GridView ID="gvTurnosTrabajo" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped" EmptyDataText="No se encontraron resultados." OnRowCommand="gvTurnosTrabajo_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Descripción" DataField="Descripcion" />
                <asp:TemplateField HeaderText="Entrada">
                    <ItemTemplate>
                        <%# ((TimeSpan)Eval("HoraEntrada")).ToString(@"hh\:mm") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Salida">
                    <ItemTemplate>
                        <%# ((TimeSpan)Eval("HoraSalida")).ToString(@"hh\:mm") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false" Text="Editar" />
                        <asp:LinkButton runat="server" CommandName='<%# (bool)Eval("Activo") ? "Baja" : "Alta" %>'
                            CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-outline-secondary" CausesValidation="false"
                            Text='<%# (bool)Eval("Activo") ? "Dar de baja" : "Reactivar" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </main>
</asp:Content>
