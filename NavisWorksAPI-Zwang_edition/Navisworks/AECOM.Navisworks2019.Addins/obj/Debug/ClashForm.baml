   M S B A M L   `   `   `  ����V  RAECOM.Navisworks2019.Addins, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:AECOM.NavisWorks2019.AddinsAECOM.NavisWorks2019.Addins  5      +    %AECOM.NavisWorks2019.Addins.ClashForm   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <static)clr-namespace:AECOM.NavisWorks2019.Addins     5        � 	Resources  5	      .   (AECOM.NavisWorks2019.Addins.categoryList   categoryList��%   &	          5
   
   64   5      .��5      � 
������� � RowDefinitions 5   
   �� 5      $��0 �6   �� 5      $	��100 �6   �� 5      $��1* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ 
selectionA��5       $��2��6      AutoGenerateColumns .     Columns 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Selection A��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6>      CellTemplate 5      �� 5      .R�5      �� ��6(   �� *cat+5      	   CellEditingTemplate	 5!      �� 5"      .R�5#      �� $��	comboBoxA��5$   '   $��22��5#   '   #��[  5%   '   
 �� SelectedItem
 5&   '   �� *cat+5(      5*      5,        5-   
   -   5        $ 
selectionB��5-      $��2��6(   $��1��65    .   5.        50      $ Selection B��6)   $ * 6>    51      �� 52      .R�53      �� ��6(   �� *cat+54      	 56      �� 57      .R�58      �� $��	comboBoxB��59   '   $��22��58   '   #��[  5:   '   
 5;   '   �� *cat+5=      5?      5A      �� 5B   
   -   5        $��0��5B      $��1��6"   $��Create Clash Test��6/   $��Stretch=�6K   $��Bottom=�6j   $
��Auto��6�   $��30��6�   $��2��6�   �� 5C   
   -   5         ��Name$ TestName��5C      $��1��6%   $��	0,40,0,30q�62   $��Bottom=�6F    �� TextWrapping$
 Wrap=�6a   $��Type test name in here��6u   $��30��6�   $��2��6�   �� 5D   
   $��Clash Test Name��6   $��1��6*   $��Bottom=�67   $��0,0,0,60q�6R   $��2��6d   $��30��6x   5F      5G      