<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW301000.aspx.cs" Inherits="Page_TW301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizations.Graph.TWNManGUIAPEntry" PrimaryView="Filter">
    <CallbackCommands>
    </CallbackCommands>
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
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
			<px:PXGridLevel DataMember="ManualGUIAP">
			    <Columns>
			      <px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
	              <px:PXGridColumn CommitChanges="True" DataField="BranchID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="VendorID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="Vatincode" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUINbr" Width="140" CommitChanges="True" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUIdate" Width="90" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="GUIDecPeriod" Width="90" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxZoneID" Width="120" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxCategoryID" Width="120" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="TaxID" Width="140" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="TaxNbr" Width="96" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="OurTaxNbr" Width="96" ></px:PXGridColumn>
                  <px:PXGridColumn DataField="Deduction" Width="70" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="NetAmt" Width="100" ></px:PXGridColumn>
                  <px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100" ></px:PXGridColumn>
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