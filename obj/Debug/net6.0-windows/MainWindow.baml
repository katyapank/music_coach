   M S B A M L   `   `   `  ����E  AOSNK-1-wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:OSNK_1_wpf
OSNK_1_wpf  5   	       OSNK_1_wpf.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:OSNK_1_wpf     5   	   _ [MaterialDesignThemes.Wpf, Version=4.8.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  Тренажёр��5	   	   $	��606��6   $	��400��6'    � 
ResizeMode$ CanMinimize=�63   .��5
      � -   5         �Name$ grid1��5
      
�������.+�5   
   �� -   5         ��Name$ targetTB��5      $��Right=�6$   $��	0,95,40,0q�6@    �� TextWrapping$
 Wrap=�6S   $�� ��6g   $	��Top=�6o   $��82��6�   $��23��6�   $��Right=�6�   �� 5   
   -   5         ��Name$ menuB��5      $��Выбрать песню��6    $��Center=�68   $��0,10,0,0q�6U   $	��Top=�6g   $	��320��6   $��45��6�    � Cursor$
 Hand��6�   �� 5   
   -   5         ��Name$ tempL��5      $��-��6   $��Right=�6+   $��
0,149,40,0q�6G   $	��Top=�6[   $��50��6s   $��82��6   $��Right=�6�   �� 5   
   -   5        $ startB��5      $��Начать запись��6!   $��Center=�69   $��	0,491,0,0q�6V   $	��Top=�6i   $	��320��6�   $��50��6�   $
 Hand��6�   �� 5   
   -   5         ��Name$ modeCB��5      $��Right=�6#   $��
0,191,40,0q�6?   $	��Top=�6S   $��81��6k   $��23��6v   	 �� SelectedIndex$	 0��6�   .@�5      �� $��Ноты��6   �� 5      $��Частоты��6   5      �� 5   
   -   5        
 ��Name$

 bord��5      
�������$��1q�63   $��	40,87,0,0q�6G   $
��Left=�6Z   $	��160��6u   $	��Top=�6�   $	��188��6�   ���	#FFE5E5E56�   �� 5   
   -   5         ��Name$ 	checkMode��5      $��Режим тюнера��6&   $��Right=�6=   $	��Top=�6Y    d� 	IsChecked$
 TrueY�6q   $��
0,454,40,0q�6�   �� 5   
   -	   5         ��Name$
 canv��5      $��0,0,0,-1q�6   63   �� 5   
   -
   5        $ lyrics��5       �� Left$ 40��6"   $
 Wrap=�63   $�� ��6G   
 �� Top$	 296��6O   $	��320��6`   $��Center=�6l   $	��Top=�6�   $	��137��6�   $��	0,304,0,0q�6�   $��16(�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�    %� 
ScrollUnit$ Pixel=�6�   �� 5   
   -   5        $ 
intervalCB��5      $��Right=�6'   $��
0,248,40,0q�6C   $	��Top=�6W   $��81��6o   $��23��6z   $	 0��6�   .@�5      �� $��Октава��6   �� 5      $!��Большая терция��6   �� 5      $��Малая терция��6   �� 5      $��Кварта��6   �� 5      $��Квинта��6   5      5      5      