<%@ Page Title="Turnos de trabajo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TurnosTrabajo.aspx.cs" Inherits="Web.TurnosTrabajo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:GridView ID="gvTurnosTrabajo" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped">
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
            </Columns>
        </asp:GridView>
    </main>
</asp:Content>
