   M S B A M L   `   `   `  ����  
MemoryGame$    MemoryGame.Views.GameSetupView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
Game Setup��5   	   $	��500��6   $	��300��6)      WindowBackground#���   5   	     WindowForeground#���  5   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��10q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   ��  �� Orientation$ Vertical=�6   $��0��6,   .+�5      �� $��Select Level:��6     TextBlockForeground#���  5      �� 5       �� SelectedValuePath$ Content��6?   $	��0,5q�6[   
�������
���   � �� SelectedValue 6   �� *SelectedLevel+.@�5      �� $��	Baby Mode��6   �� 5      $��	Easy Mode��6   �� 5      $��Medium Mode��6   �� 5      $��Intermediate Mode��6   �� 5      $��	Hard Mode��6   �� 5      $��Very Hard Mode��6   �� 5      $��Challenging Mode��6   �� 5      $��Expert Mode��6   �� 5      $��Nightmare Mode��6   �� 5      $��	Hell Mode��6   �� 5      $��Insane Mode��6   �� 5       $��God Mode��6   �� 5!      $��Custom Mode��6   5"      �� 5$      $��Select Module:��6   #���  5%      �� 5&      $ Content��6@   $	��0,5q�6\   
�������
���   � 6   �� *SelectedModule+.@�5(      �� $��Animals��6   �� 5)      $��Flowers��6   �� 5*      $��Trees��6   �� 5+      $��
Vegetables��6   �� 5,      $��Fruits��6   �� 5-      $��Rocks��6   �� 5.      $��
Landscapes��6   �� 5/      $��	Buildings��6   �� 50      $��
Motorcycle��6   �� 51      $
��Cars��6   �� 52      $��Tools��6   �� 53      $��Random��6   54      �� 56      $ 
Horizontal=�6   .+�57      �� $��Rows:��6   $	��0,5q�6)   #���  66   �� 58      $��30��6     TextBoxBackground#���  59        TextBoxForeground#���  6K   ��58   %   �� *Rows+{�6;   �� *IsSizeEditable+59   K   �� 5:      $��Cols:��6   $��10,5,0,5q�6)   #���  6;   �� 5;      $��51��6   #���  5<      #���  6K   ��5;   %   �� *	Columns+{�6>   �� *IsSizeEditable+5<   K   5=      �� 5?      $ 
Horizontal=�6   .+�5@      �� $��Time (sec):��6   $	��0,5q�6/   #���  6<   �� 5A      $��50��6   #���  5B      #���  6K   ��5A   %   �� *Time+{�6;   �� *IsTimeEditable+5B   K   5C      5D      �� 5G   
   $ 
Horizontal=�6   $��2��6.   $��Center=�6;   $��0,10,0,0q�6X   .+�5H      �� $
��Back��6   $��60��6$   $��5q�6/     ButtonStyle#���  5K      ��5I      �� *BackCommand+��5J      ��  �� RelativeSource ��  �� AncestorType
 Window5K      �� 5L      $
��Play��6   $��60��6$   $��5q�6/   #���  5O      ��5M      �� *PlayCommand+��5N      ��  �� 
 Window5O      �� 5P      $��Continue��6   $��80��6(   $��5q�63   #���  5S      ��5Q      �� *ContinueCommand+��5R      ��  �� 
 Window5S      5U      �� 5V   
   $ 
Horizontal=�6   $��3��6.   $��Center=�6;   $��0,10,0,0q�6X   .+�5W      �� $��Clear Saved Game��6   $	��150��60   $��5q�6<   #���  5Z      ��5X      �� *ClearSavedGameCommand+��5Y      ��  �� 
 Window5Z      5[      5]      5^      