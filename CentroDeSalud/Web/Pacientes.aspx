<%@ Page Title="Pacientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pacientes.aspx.cs" Inherits="Web.Pacientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:GridView ID="gvPacientes" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped">
            <Columns>
                <asp:BoundField HeaderText="Apellido" DataField="Apellido" />
                <asp:BoundField HeaderText="Nombre" DataField="Nombre" />
                <asp:BoundField HeaderText="DNI" DataField="Dni" />
                <asp:BoundField HeaderText="Nacimiento" DataField="FechaNacimiento" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField HeaderText="Email" DataField="Email" />
                <asp:BoundField HeaderText="Teléfono" DataField="Telefono" NullDisplayText="-" />
                <asp:BoundField HeaderText="Dirección" DataField="Direccion" NullDisplayText="-" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </main>
</asp:Content>
