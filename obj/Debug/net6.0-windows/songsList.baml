   M S B A M L   `   `   `  ����E  AOSNK-1-wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:OSNK_1_wpf
OSNK_1_wpf  5   	       OSNK_1_wpf.songsList   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:OSNK_1_wpf     5   	   _ [MaterialDesignThemes.Wpf, Version=4.8.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$!  Выберите песню��5	   	   $	��450��6    $	��320��6-    � ShowInTaskbar$ False��69    � 
ResizeMode$ CanMinimize=�6O    � Topmost$
 True��6h    � WindowStartupLocation$ CenterOwner=�6w    � WindowStyle$ 
ToolWindow=�6�   .��5
      � -   5         �Name$ grid2��5
      .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ songL��5         
IsReadOnly$
 True��6B   	   AutoGenerateColumns$	 False��6T   
   Columns
 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Песня��6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *Song+6>     5      $ Сложность��6?    6%   �� *Level+6?   5      5      5      