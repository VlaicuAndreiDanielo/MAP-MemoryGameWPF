   M S B A M L   `   `   `  ����  
MemoryGame    MemoryGame.Views.GameView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Memory Game��5   	   $	��600��6   $	��800��6*    � WindowStartupLocation$ CenterScreen=�5   	      WindowBackground#���   5   	     WindowForeground#���  5   	   .��5      �  � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��Center=�5      $��Center=�5      $��16(�5      $
��Bold"�6&   
���  ����5      �� *TimeRemaining+5   8   �� 5      $��Pause��6   $��1��5      $	��5,0q�6%     ButtonStyle#���  5      ��5   %   �� *PauseCommand+5      5      �� 5   
   $��1��6   ��6$   �� *Cards+��5      �� 5      .R�5      A�  A� Rows 6"   �� *DataContext.Rows+ �� RelativeSource ��  �� AncestorType
 Window A� Columns 5    "   �� *DataContext.Columns+ �� 
 Window5!      ��5#      �� 5$      .R�5%      �� 
���   �$��2q�5'   1   
���������5%      �� *DataContext.CardFlipCommand+ �� 
 Window��5&      �� ��5)      �� 5*       �� 
TargetType" ��6.   .R�5+   "   � .+�5,   &   �� 	 ��Name$	 	CardImage��6,   $��Uniform=�6?   5-   #   
 �� Triggers
 5.   "   �� 5/   &    �� Value$ False��6P    �� Binding 62   �� *	ShowFront+.a�50   *   ��  �� 
TargetName$ 	CardImage��61    �� Value 6Z   �� *
FrontImage+ �� Property � ��51   '   �� 52   &   $
 True��6P    62   �� *	ShowFront+.a�53   *   �� $ 	CardImage��61    6Z   �� *	BackImage+ � ��54   '   56      58      59      5;      5<      5=      