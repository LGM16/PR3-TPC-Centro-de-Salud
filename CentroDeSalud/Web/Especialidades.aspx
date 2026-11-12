<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="Web.Especialidades" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" ClientIDMode="Static" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <div class="row g-2 align-items-end mb-4">
            <div class="col-md-6">
                <asp:HiddenField ID="hfId" runat="server" Value="0" />
                <asp:Label ID="lblFormulario" runat="server" AssociatedControlID="txtDescripcion" CssClass="form-label" Text="Nueva especialidad"></asp:Label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server" ControlToValidate="txtDescripcion"
                    ErrorMessage="La descripción es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" OnClientClick="var m = document.getElementById('lblMensaje'); if (m) m.style.display = 'none';" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnCancelar_Click" />
            </div>
        </div>

        <asp:GridView ID="gvEspecialidades" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped" OnRowCommand="gvEspecialidades_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Id" DataField="Id" />
                <asp:BoundField HeaderText="Descripción" DataField="Descripcion" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <%# (bool)Eval("Activo") ? "Activa" : "Inactiva" %>
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
