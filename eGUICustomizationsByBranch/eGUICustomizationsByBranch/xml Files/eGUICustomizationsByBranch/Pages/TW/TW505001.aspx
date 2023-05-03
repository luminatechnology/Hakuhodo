<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW505001.aspx.cs" Inherits="Page_TW505001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizations.Graph.TWNGenWHTFile_New" PrimaryView="Filter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector1" DataField="BranchID" CommitChanges="true"></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit1" CommitChanges="True" DataField="FromPaymDate" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit2" CommitChanges="True" DataField="ToPaymDate" ></px:PXDateTimeEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false" NoteIndicator="false" FilesIndicator="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTranProcNew">
			    <Columns>
					<px:PXGridColumn DataField="Selected" Width="30" Type="CheckBox" AllowCheckAll="True" TextAlign="Center" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BranchID" Width="140" />
					<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TranDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="PaymDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="PersonalID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="PropertyID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TypeOfIn" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="WHTFmtCode" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="WHTFmtSub" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="PayeeName" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="PayeeAddr" Width="220" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CountryID" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="SecNHICode" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="SecNHIPct" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="SecNHIAmt" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="WHTTaxPct" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>