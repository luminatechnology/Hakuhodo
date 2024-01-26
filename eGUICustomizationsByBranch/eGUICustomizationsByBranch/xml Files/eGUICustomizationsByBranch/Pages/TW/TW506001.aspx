<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW506001.aspx.cs" Inherits="Page_TW506001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="eGUICustomizationsByBranch.Graph.TWNGenNHIFile_65" PrimaryView="Filter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="125px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="PXSelector1" DataField="BranchID" CommitChanges="true"></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit1" CommitChanges="True" DataField="FromDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit2" CommitChanges="True" DataField="ToDate" ></px:PXDateTimeEdit>
			<px:PXMultiSelector runat="server" ID="PXSelector2" DataField="SecNHICode"> </px:PXMultiSelector>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDropDown runat="server" ID="PXDropDown1" DataField="ProcessingMethod" Size="S" ></px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false" NoteIndicator="false" FilesIndicator="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTran">
			    <Columns>
			        <px:PXGridColumn DataField="Selected" Width="30" Type="CheckBox" AllowCheckAll="True" TextAlign="Center" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BatchNbr" Width="140" />
					<px:PXGridColumn DataField="RefNbr" Width="140" />
					<px:PXGridColumn DataField="BranchID" Width="140" />
					<px:PXGridColumn DataField="PaymDate" Width="140" />
					<px:PXGridColumn DataField="PersonalID" Width="140" />
					<px:PXGridColumn DataField="PayeeName" Width="140" />
					<px:PXGridColumn DataField="INetAmt" Width="140" />
					<px:PXGridColumn DataField="ISecNHIAmt" Width="140" />
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar PagerVisible="Bottom" >
			<PagerSettings Mode="NumericCompact" />
		</ActionBar>
	</px:PXGrid>
</asp:Content>