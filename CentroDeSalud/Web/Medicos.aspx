<%@ Page Title="Médicos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Medicos.aspx.cs" Inherits="Web.Medicos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title" class="mt-3"><%: Title %></h2>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block"></asp:Label>

        <asp:HiddenField ID="hfId" runat="server" Value="0" />
        <asp:Label ID="lblFormulario" runat="server" CssClass="form-label fw-bold d-block" Text="Nuevo médico"></asp:Label>
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
                <asp:Label runat="server" AssociatedControlID="txtMatricula" CssClass="form-label" Text="Matrícula"></asp:Label>
                <asp:TextBox ID="txtMatricula" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMatricula" runat="server" ControlToValidate="txtMatricula"
                    ErrorMessage="La matrícula es obligatoria." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-4">
                <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="form-label" Text="Email (opcional)"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="El email no tiene un formato válido." CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="txtTelefono" CssClass="form-label" Text="Teléfono (opcional)"></asp:Label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="ddlTurno" CssClass="form-label" Text="Turno de trabajo"></asp:Label>
                <asp:DropDownList ID="ddlTurno" runat="server" CssClass="form-select"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvTurno" runat="server" ControlToValidate="ddlTurno" InitialValue="0"
                    ErrorMessage="Elegí un turno de trabajo." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-12">
                <asp:Label runat="server" AssociatedControlID="cblEspecialidades" CssClass="form-label" Text="Especialidades (al menos una)"></asp:Label>
                <asp:CheckBoxList ID="cblEspecialidades" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-flex flex-wrap gap-3"></asp:CheckBoxList>
            </div>
            <div class="col-12">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnCancelar_Click" />
            </div>
        </div>

        <asp:Panel runat="server" DefaultButton="btnFiltrar" CssClass="row g-2 mb-3">
            <div class="col-md-6">
                <asp:TextBox ID="txtFiltro" runat="server" CssClass="form-control" placeholder="Buscar por nombre, apellido o especialidad..."></asp:TextBox>
            </div>
            <div class="col-md-6">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-primary" CausesValidation="false" OnClick="btnFiltrar_Click" />
                <asp:Button ID="btnLimpiarFiltro" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="btnLimpiarFiltro_Click" />
            </div>
        </asp:Panel>

        <asp:GridView ID="gvMedicos" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            CssClass="table table-striped" EmptyDataText="No se encontraron resultados." OnRowCommand="gvMedicos_RowCommand">
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
