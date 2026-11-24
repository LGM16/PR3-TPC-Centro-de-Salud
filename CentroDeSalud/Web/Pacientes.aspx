<%@ Page Title="Pacientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pacientes.aspx.cs" Inherits="Web.Pacientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:HiddenField ID="hfId" runat="server" Value="0" />
        <asp:Label ID="lblFormulario" runat="server" CssClass="form-label fw-bold d-block" Text="Nuevo paciente"></asp:Label>
        <div class="row g-2 mb-4">
            <div class="col-md-4">
                <asp:Label runat="server" AssociatedControlID="txtNombre" CssClass="form-label" Text="Nombre"></asp:Label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre"
                    ErrorMessage="El nombre es obligatorio." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4">
                <asp:Label runat="server" AssociatedControlID="txtApellido" CssClass="form-label" Text="Apellido"></asp:Label>
                <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvApellido" runat="server" ControlToValidate="txtApellido"
                    ErrorMessage="El apellido es obligatorio." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" AssociatedControlID="txtDni" CssClass="form-label" Text="DNI"></asp:Label>
                <asp:TextBox ID="txtDni" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDni" runat="server" ControlToValidate="txtDni"
                    ErrorMessage="El DNI es obligatorio." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revDni" runat="server" ControlToValidate="txtDni"
                    ValidationExpression="^\d{7,8}$" ErrorMessage="El DNI debe tener 7 u 8 números." CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" AssociatedControlID="txtFecha" CssClass="form-label" Text="Nacimiento"></asp:Label>
                <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFecha" runat="server" ControlToValidate="txtFecha"
                    ErrorMessage="La fecha es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4">
                <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="form-label" Text="Email"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="El email es obligatorio." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="El email no tiene un formato válido." CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="txtTelefono" CssClass="form-label" Text="Teléfono (opcional)"></asp:Label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
            </div>
            <div class="col-md-5">
                <asp:Label runat="server" AssociatedControlID="txtDireccion" CssClass="form-label" Text="Dirección (opcional)"></asp:Label>
                <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
            </div>
            <div class="col-12">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnCancelar_Click" />
            </div>
        </div>

        <asp:GridView ID="gvPacientes" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped" OnRowCommand="gvPacientes_RowCommand">
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
