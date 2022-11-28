<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW205000.aspx.cs" Inherits="Page_TW205000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizations.Graph.TWNPrintedLineDetMaint" PrimaryView="Filter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="70px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True"/>
			<px:PXSelector ID="PXSelector2" runat="server" DataField="GUIFormatCode" CommitChanges="true" DisplayMode="Hint"></px:PXSelector>
			<px:PXSelector ID="PXSelector3" runat="server" DataField="RefNbr" CommitChanges="true" AutoRefresh="true"></px:PXSelector>
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True"/>
			<px:PXLabel ID="CustPXLabel" runat="server" ></px:PXLabel>
			<px:PXSelector ID="PXSelector1" runat="server" DataField="GUINbr" CommitChanges="true" AutoRefresh="true"></px:PXSelector>
			<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True"/>
			<px:PXNumberEdit ID="PXNumberEdit1" runat="server" DataField="OrigAmount"></px:PXNumberEdit>
			<px:PXNumberEdit ID="PXNumberEdit2" runat="server" DataField="RevisedAmount"></px:PXNumberEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false" NoteIndicator="false" FilesIndicator="false">
		<CallbackCommands>
			<Refresh RepaintControlsIDs="form" />
		</CallbackCommands>
		<Levels>
			<px:PXGridLevel DataMember="PrintedLineDet">
			    <Columns>
					<px:PXGridColumn DataField="GUIFormatCode" Width="60"></px:PXGridColumn>
					<px:PXGridColumn DataField="RefNbr" Width="120"></px:PXGridColumn>
			        <px:PXGridColumn DataField="GUINbr" Width="120"></px:PXGridColumn>
					<px:PXGridColumn DataField="LineNbr" Width="80"></px:PXGridColumn>
					<px:PXGridColumn DataField="Descr" Width="260"></px:PXGridColumn>
					<px:PXGridColumn DataField="Qty" Width="120" CommitChanges="true"></px:PXGridColumn>
					<px:PXGridColumn DataField="UnitPrice" Width="120" CommitChanges="true"></px:PXGridColumn>
					<px:PXGridColumn DataField="Amount" Width="120" CommitChanges="true"></px:PXGridColumn>
					<px:PXGridColumn DataField="Remark" Width="230"></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar PagerVisible="Bottom" >
			<PagerSettings Mode="NumericCompact" />
		</ActionBar>
		<Mode AllowUpload="true" />
	</px:PXGrid>
</asp:Content>