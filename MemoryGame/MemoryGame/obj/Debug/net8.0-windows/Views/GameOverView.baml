   M S B A M L   `   `   `  ����  
MemoryGame#    MemoryGame.Views.GameOverView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  	Game Over��5   	   $	��300��6   $	��400��6(    � WindowStartupLocation$ CenterScreen=�5   	      WindowBackground#���   5   	     WindowForeground#���  5   	   .��5      � $��10q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Ai pierdut!��6   $��24(�5      $
��Bold"�5      $��Center=�5      $
��0,10q�5        TextBlockForeground#���  5      �� 5   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $��0,10,0,0q�6X   .+�5      �� $��Retry��6   $��80��6%   $��5q�60     ButtonStyle#���  5      ��5      �� *RetryCommand+��5      ��  �� RelativeSource ��  �� AncestorType
 Window5      �� 5      $��Restart��6   $��80��6'   $��5q�62   #���  5      ��5      �� *RestartCommand+��5      ��  �� 
 Window5      �� 5      $
��Quit��6   $��80��6$   $��5q�6/   #���  5       ��5      �� *QuitCommand+��5      ��  �� 
 Window5       5!      5"      5#      