   M S B A M L   `   `   `  ����E  AMemoryGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:MemoryGame.ConvertersMemoryGame.Converters  5   	   =#clr-namespace:MemoryGame.ViewModelsMemoryGame.ViewModels  5   	   %    MemoryGame.Views.CreateUserView   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4conv#clr-namespace:MemoryGame.Converters     5   	   2vm#clr-namespace:MemoryGame.ViewModels     5   	     � Title$  Create New User��5   	   $	��450��5   	   $	��300��6    � WindowStartupLocation$ CenterScreen=�5   	      WindowBackground#���   5	   	     WindowForeground#���  5
   	    � DataContext 5      /   )MemoryGame.ViewModels.CreateUserViewModel  5   
    � 	Resources 5      8   2MemoryGame.Converters.StringToImageSourceConverter   StringToImageSourceConverter��%   &	         5   
   6,   5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��0,0,0,10q�6;   .+�5      �� $��
User Name:��6   $��Center=�6)     TextBlockForeground#���  5      �� 5      $	��176��6   $	��5,0q�6"     TextBoxBackground#���  5         TextBoxForeground#���  5!      ��5      �� *
UserName+ �� UpdateSourceTrigger$ PropertyChanged=�5!      5"      �� 5$   
   $ Vertical=�6   $��1��6,   $��Center=�69   $��Center=�6V   .+�5%      �� 
���  ��$
��Bold"�5&   )   $��0,0,0,5q�6;   ��5%      �� *SelectedAvatarPath+ �� FallbackValue$  SelectedAvatarPath not set��5&   ;   �� 5'      
���   �$��2q�6)   $	��222��6=   $	��230��6J   ._�5(      �� $	��230��6   $	��222��6%   ��5)      �� 5*       j� Stretch$ UniformToFill=�5+   %   	 �� ImageSource	 5*   %   �� *SelectedAvatarPath+
 �� 	Converter#
 [ 5+   %   5-      5.      �� 5/      $ 
Horizontal=�6   $��Center=�62   $��0,10,0,0q�6O   .+�50      �� $��<<��6   $��40��6Q   $	��5,0q�6\     ButtonStyle#���  51      ��50   ,   �� *PrevImageCommand+51      �� 52      $��>>��6   $��40��6Q   $	��5,0q�6\   #���  53      ��52   ,   �� *NextImageCommand+53      54      55      �� 57   
   $ 
Horizontal=�6   $��2��6.   $��Center=�6;   $��	0,10,0,10q�6X   $	��260��6k   .+�58      �� $��Create��6   $	��119��6&   $��5q�62   #���  5;      ��59      �� *CreateCommand+��5:      ��  �� RelativeSource ��  �� AncestorType
 Window5;      �� 5<      $��Cancel��6   $	��122��6&   $��5q�62   #���  5?      ��5=      �� *CancelCommand+��5>      ��  �� 
 Window5?      5@      5A      5B      