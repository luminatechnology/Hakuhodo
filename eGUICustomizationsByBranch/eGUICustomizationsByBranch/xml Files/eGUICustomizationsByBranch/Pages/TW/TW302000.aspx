<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW302000.aspx.cs" Inherits="Page_TW302000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizations.Graph.TWNManGUIAREntry" PrimaryView="Filter">
    <CallbackCommands>
	<%--<px:PXDSCallbackCommand Text="GUI Invoice" Name="PrintGUIInvoice" Visible="False" DependOnGrid="CstPXGrid2" CommitChanges="true" HideText="False" ></px:PXDSCallbackCommand>--%>
    </CallbackCommands>
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="45px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"/>
            <px:PXDropDown runat="server" ID="CstPXDropDown1" DataField="Status" CommitChanges="true" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false" SyncPosition="true">
		<Levels>
			<px:PXGridLevel DataMember="ManualGUIAR">
			    <Columns>
			      <px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="BranchID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="CustomerID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="VatOutCode" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUINbr" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUIDate" Width="90" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxZoneID" Width="120" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxCategoryID" Width="120" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="TaxNbr" Width="96" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="OurTaxNbr" Width="96" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="NetAmt" Width="100" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUITitle" Width="140" ></px:PXGridColumn>
				  <px:PXGridColumn DataField="AddressLine" Width="180" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="CustomType" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="ExportMethod" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="ExportTicketType" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="ExportTicketNbr" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="ClearingDate" Width="90" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="CreatedByID" Width="220" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="Remark" Width="140" ></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <Mode AllowUpload="true" ></Mode>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>