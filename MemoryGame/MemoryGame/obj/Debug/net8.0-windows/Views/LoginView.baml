   M S B A M L   `   `   `  ����E  AMemoryGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:MemoryGame.ConvertersMemoryGame.Converters  5   	   =#clr-namespace:MemoryGame.ViewModelsMemoryGame.ViewModels  5   	        MemoryGame.Views.LoginView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4conv#clr-namespace:MemoryGame.Converters     5   	   2vm#clr-namespace:MemoryGame.ViewModels     5   	     � Title$  Sign In��5   	   $	��370��6   $	��450��6&    � WindowStartupLocation$ CenterScreen=�5   	      WindowBackground#���   5   	     WindowForeground#���  5	   	    � DataContext 5      *   $MemoryGame.ViewModels.LoginViewModel  5   
    � 	Resources 5      /   )MemoryGame.Converters.NullToBoolConverter  NullToBoolConverter8   2MemoryGame.Converters.StringToImageSourceConverter   StringToImageSourceConverter��%:   &	       &	        5   
   6#     5   
   6,   5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5    
   ��  �� Orientation$ 
Horizontal=�6   $
��Left=�5!      $��0,0,0,5q�5"      $��0��5#      $��2��5$      .+�5%      �� $
��Help��6   $��60��6$   $��0,0,5,0q�6/     ButtonStyle#���  5'      ��5&      �� *HelpCommand+5'      �� 5(      $��
Statistics��6   $��80��6*   $��0,0,5,0q�65   #���  5*      ��5)      �� *StatisticsCommand+5*      �� 5+      $��Theme��6   $��80��6%   $��0,0,5,0q�60   #���  5-      ��5,      �� *ThemeCommand+5-      5.      �� 50   
   $��Wellcome Enjoy this game!��6   $��16(�51      $
��Bold"�52        TextBlockForeground#���  53      $��Center=�54      $��0,0,0,10q�55      $��1��56      $��2��6!   �� 58   
   -   5         ��Name$ UserList��58       �� DisplayMemberPath$
 Name��5;      $	��120��5<      $��2��5=      $��0��6   $��0,0,10,0q�5>        TextBoxBackground#���  5?        TextBoxForeground#���  5@      ��59      �� *Users+	 �� SelectedItem	 5:      �� *SelectedUser+5@      �� 5B   
   $��2��6   $��1��6"   $��Center=�5C      $��Center=�62   .+�5D      �� $	��240��6   $	��218��6!   $��0,0,0,10q�6.   ��5E      �� 5F      
 j� Stretch$
 Uniform=�5G   !    �� ImageSource 5F   !   �� *SelectedUser.ImagePath+ �� 	Converter# [ 5G   !   5I      5J      �� 5L   
   $��3��6   $��2��6"   $ 
Horizontal=�5M      $��Center=�5N      $��0,10,0,0q�5O      .+�5P      �� $��
 New User ��6   $	��100��5S      $	��5,0q�6!   #���  5T      ��5Q      �� *CreateUserCommand+��5R      �� 5T      �� 5U      $��Delete User��6   $	��110��5Y      $	��5,0q�6!   #���  5Z      ��5V      �� *DeleteUserCommand+{�5W      �� *SelectedUser+# [ ��5X      �� *SelectedUser+5Z      �� 5[      $
��Play��6   $��80��5_      $	��5,0q�6    #���  5`      ��5\      �� *PlayCommand+{�5]      �� *SelectedUser+# [ ��5^      �� *SelectedUser+5`      �� 5c      $��Cancel��6   $��80��5e      $	��5,0q�6    #���  5f      ��5d      �� *CancelCommand+5f      5g      5h      5i      