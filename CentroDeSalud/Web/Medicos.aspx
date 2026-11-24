<%@ Page Title="Médicos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Medicos.aspx.cs" Inherits="Web.Medicos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:GridView ID="gvMedicos" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped">
            <Columns>
                <asp:BoundField HeaderText="Apellido" DataField="Apellido" />
                <asp:BoundField HeaderText="Nombre" DataField="Nombre" />
                <asp:BoundField HeaderText="DNI" DataField="Dni" />
                <asp:BoundField HeaderText="Matrícula" DataField="Matricula" />
                <asp:BoundField HeaderText="Email" DataField="Email" NullDisplayText="-" />
                <asp:BoundField HeaderText="Teléfono" DataField="Telefono" NullDisplayText="-" />
                <asp:BoundField HeaderText="Turno" DataField="TurnoTrabajo.Descripcion" />
                <asp:TemplateField HeaderText="Especialidades">
                    <ItemTemplate>
                        <%# unirEspecialidades(Eval("Especialidades")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </main>
</asp:Content>
