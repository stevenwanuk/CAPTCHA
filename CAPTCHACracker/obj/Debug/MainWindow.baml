   M S B A M L   `   `   `  ����I  ECAPTCHACracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:CAPTCHACrackerCAPTCHACracker  5   	       CAPTCHACracker.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:CAPTCHACracker     5   	     � Title$  
MainWindow��5   	   .��5	      �� .+�5
   
   �� -   5         ��Name$ 	OriginImg��5
   ,   $
��Left=�6   $
��None=�6_   ��6?   �� *OriginBitmap+6q   �� 5   
   -   5        .��5   +   	PreHandle66   �� 5   
   -   5        $ BppImg��5      $
��None=�6    61   �� 5   
   -   5        $ GrayImg��5      $
��None=�6!   62   �� 5   
   -   5        $ ThresholdImg��5      $
��None=�6&   67   �� 5   
   -   5        $ DoneImg��5      $
��None=�6!   62   �� 5   
   -   5        .��5   -   SplitCharcter6<   � 5   
    � ColumnDefinitions 5      �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $��* �6#   6/   .+�5       �� -   5        $ Split1��5       $
��None=�6$   $��0��63   6E   �� 5"      -	   5        $ Split2��5"      $
��None=�6$   $��3��64   6F   �� 5$      -
   5        $ Split3��5$      $
��None=�6$   $��5��64   6F   �� 5&      -   5        $ Split4��5&      $
��None=�6$   $��7��64   6F   �� 5(      -   5        $ Split5��5(      $
��None=�6$   $��9��64   6F   �� 5*      -   5        $ Split6��5*      $
��None=�6$   $��11��64   6G   5+      �� 5,   
   -   5         ��Name$ TBlock��5,      6&   5.      50      