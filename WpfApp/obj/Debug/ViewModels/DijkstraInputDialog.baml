   M S B A M L   `   `   `  ����
  WpfApp&     WpfApp.Views.DijkstraInputDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$"  Select Vertices for Dijkstra��5   	   $	��150��6.   $	��300��6;    � WindowStartupLocation$ CenterScreen=�6G   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0,0,0,10q�6.   .+�5      �� $��Start Vertex ID:��6   $��80��60   �� 5      -   5         ��Name$ StartVertexComboBox��5      $	��150��64   5      �� 5   
   $ 
Horizontal=�6   $��1��6.   $��0,0,0,10q�6;   .+�5      �� $��End Vertex ID:��6   $��80��6.   �� 5      -   5        $ EndVertexComboBox��5      $	��150��62   5      �� 5   
   $ 
Horizontal=�6   $��2��6.   $��Right=�6;   .+�5      �� -   5        $��OK��5      $��75��6"   $��0,0,10,0q�6-   6?   �� 5      -   5        $��Cancel��5      $��75��6&   61   5      5      5      