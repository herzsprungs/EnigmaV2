using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TRYINGSMT
{
    
    public partial class MainWindow : Window
    {
        
        string _control = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string _ring1 = "DMTWSILRUYQNKFEJCAZBPGXOHV";
        string _ring2 = "HQZGPJTMOBLNCIFDYAWVEUSRKX";
        string _ring3 = "UQNTLSZFMREHDPXKIBVYGJCWOA";
        string _reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
        int[] _keyOffset = { 0, 0, 0 };
        int[] _initOffset = { 0, 0, 0 };
        bool _rotor = false;
        Dictionary<char, char> _plugboard = new Dictionary<char, char>();
        private bool _plugboardSet = false;
        private TextBox _activeTextBox;
        private int _previousInputLength = 0;

        public MainWindow()
        {
            InitializeComponent();
            SetDefaults();
            _rotor = false;
            btnRotor.Content = "Rotor On";
            btnRotor.IsEnabled = false;

            
            LetterQ.Click += VISUALkeyboard;
            LetterW.Click += VISUALkeyboard;
            LetterE.Click += VISUALkeyboard;
            LetterR.Click += VISUALkeyboard;
            LetterT.Click += VISUALkeyboard;
            LetterY.Click += VISUALkeyboard;
            LetterU.Click += VISUALkeyboard;
            LetterI.Click += VISUALkeyboard;
            LetterO.Click += VISUALkeyboard;
            LetterP.Click += VISUALkeyboard;
            LetterA.Click += VISUALkeyboard;
            LetterS.Click += VISUALkeyboard;
            LetterD.Click += VISUALkeyboard;
            LetterF.Click += VISUALkeyboard;
            LetterG.Click += VISUALkeyboard;
            LetterH.Click += VISUALkeyboard;
            LetterJ.Click += VISUALkeyboard;
            LetterK.Click += VISUALkeyboard;
            LetterL.Click += VISUALkeyboard;
            LetterZ.Click += VISUALkeyboard;
            LetterX.Click += VISUALkeyboard;
            LetterC.Click += VISUALkeyboard;
            LetterV.Click += VISUALkeyboard;
            LetterB.Click += VISUALkeyboard;
            LetterN.Click += VISUALkeyboard;
            LetterM.Click += VISUALkeyboard;
            ButtonBackspace.Click += ButtonBackspace_Click;
            ButtonSpace.Click += ButtonSpace_Click;
            txtInput.GotFocus += TextBox_GotFocus;
            txtPlugboard.GotFocus += TextBox_GotFocus;
            txtBRing1Init.GotFocus += TextBox_GotFocus;
            txtBRing2Init.GotFocus += TextBox_GotFocus;
            txtBRing3Init.GotFocus += TextBox_GotFocus;
        }

        private void ButtonSpace_Click(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null)
            {
                _activeTextBox.Text += " ";
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length;
            }
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null && _activeTextBox.Text.Length > 0)
            {
                _activeTextBox.Text = _activeTextBox.Text.Substring(0, _activeTextBox.Text.Length - 1);
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length;
            }
        }

        private void VISUALkeyboard(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null)
            {
                Button clickedButton = (Button)sender;
                _activeTextBox.Text += clickedButton.Content.ToString();
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _activeTextBox = (TextBox)sender;
        }

        private void ROTORdisplay(Label displayLabel, string ring)
        {
            int length = ring.Length;
            int columns = (int)Math.Ceiling(Math.Sqrt(length));
            string temp = "";

            for (int i = 0; i < length; i++)
            {
                temp += ring[i] + "\t";
                if ((i + 1) % columns == 0)
                {
                    temp += "\n";
                }
            }

            displayLabel.Content = temp;
        }

        private int IndexSearch(string ring, char letter)
        {
            int index = 0;
            for (int x = 0; x < ring.Length; x++)
            {
                if (ring[x] == letter)
                {
                    index = x;
                    break;
                }
            }
            return index;
        }


        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = txtInput.Text.ToUpper();

            if (input.Length < _previousInputLength)
            {
               
                if (txtEncrypt.Text.Length > 0 && txtEncryptMirror.Text.Length > 0)
                {
                    txtEncrypt.Text = txtEncrypt.Text.Substring(0, txtEncrypt.Text.Length - 1);
                    txtEncryptMirror.Text = txtEncryptMirror.Text.Substring(0, txtEncryptMirror.Text.Length - 1);
                }
            }
            else if (input.Length > _previousInputLength)
            {
                char newChar = input[_previousInputLength];

                if (newChar == ' ')
                {
                    txtEncrypt.Text += " "; 
                    txtEncryptMirror.Text += " ";
                }
                else if (char.IsLetter(newChar))
                {
                   
                    txtEncrypt.Text += Encrypt(newChar);
                    txtEncryptMirror.Text += Mirror(newChar);

                   
                    if (_rotor)
                    {
                        Rotate(true);
                    }
                }
                else
                {
                    
                    txtInput.Text = RemoveLastLetter(input);
                    txtInput.CaretIndex = txtInput.Text.Length;
                    MessageBox.Show("Only letters and spaces are allowed!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            
            _previousInputLength = txtInput.Text.Length;
        }

        private char Encrypt(char letter)
        {
            char newChar = letter;

           
            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

            
            newChar = _ring1[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring3[IndexSearch(_control, newChar)];

            
            newChar = _reflector[IndexSearch(_control, newChar)];

            
            newChar = _ring3[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];

         
            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

            return newChar;
        }

        private char Mirror(char letter)
        {
            char newChar = Encrypt(letter);

            newChar = _ring3[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];

            return newChar;
        }

        private void SetDefaults()
        {
            _control = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            _ring1 = "DMTWSILRUYQNKFEJCAZBPGXOHV";
            _ring2 = "HQZGPJTMOBLNCIFDYAWVEUSRKX";
            _ring3 = "UQNTLSZFMREHDPXKIBVYGJCWOA";
            _reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
            _keyOffset = new int[] { 0, 0, 0 };

            txtInput.Text = "";
            txtEncrypt.Text = "";
            txtEncryptMirror.Text = "";

            ROTORdisplay(lblControlRing, _control);
            ROTORdisplay(lblRing1, _ring1);
            ROTORdisplay(lblRing2, _ring2);
            ROTORdisplay(lblRing3, _ring3);
            ROTORdisplay(ReflectorLabel, _reflector);
        }

        private void Rotate(bool forward)
        {
            if (forward)
            {
                _keyOffset[2]++;
                _ring3 = MoveValues(forward, _ring3);

                if (_keyOffset[2] / _control.Length >= 1)
                {
                    _keyOffset[2] = 0;
                    _keyOffset[1]++;
                    _ring2 = MoveValues(forward, _ring2);
                    if (_keyOffset[1] / _control.Length >= 1)
                    {
                        _keyOffset[1] = 0;
                        _keyOffset[0]++;
                        _ring1 = MoveValues(forward, _ring1);
                    }
                }
            }
            else
            {
                if (_keyOffset[2] > 0)
                {
                    _keyOffset[2]--;
                    _ring3 = MoveValues(forward, _ring3);
                    if (_keyOffset[2] < 0)
                    {
                        _keyOffset[2] = 25;
                        _keyOffset[1]--;
                        _ring2 = MoveValues(forward, _ring2);
                        if (_keyOffset[1] < 0)
                        {
                            _keyOffset[1] = 25;
                            _keyOffset[0]--;
                            _ring1 = MoveValues(forward, _ring1);
                            if (_keyOffset[0] < 0)
                                _keyOffset[0] = 25;
                        }
                    }
                }
            }
           
            ROTORdisplay(lblRing1, _ring1);
            ROTORdisplay(lblRing2, _ring2);
            ROTORdisplay(lblRing3, _ring3);
            DisplayOffset();
           
        }
        private string MoveValues(bool forward, string ring)
        {
            char movingValue = ' ';
            string newRing = "";

            if (forward)
            {
                movingValue = ring[0];
                for (int x = 1; x < ring.Length; x++)
                    newRing += ring[x];
                newRing += movingValue; 
            }
            else
            {
                movingValue = ring[25];
                for (int x = 0; x < ring.Length - 1; x++)
                    newRing += ring[x];
                newRing = movingValue + newRing; 
            }

            return newRing;
        }
        private void btnRotor_Click(object sender, RoutedEventArgs e)
        {
            SetDefaults();

            
            if (!ValidateRotorInput(txtBRing1Init.Text, 0) ||
                !ValidateRotorInput(txtBRing2Init.Text, 1) ||
                !ValidateRotorInput(txtBRing3Init.Text, 2))
            {
                return; 
            }

           
            txtBRing1Init.IsEnabled = false;
            txtBRing2Init.IsEnabled = false;
            txtBRing3Init.IsEnabled = false;

            _rotor = true;
            btnRotor.Content = "Settings Lock";

            _ring1 = InitializeRotors(_initOffset[0], _ring1);
            _ring2 = InitializeRotors(_initOffset[1], _ring2);
            _ring3 = InitializeRotors(_initOffset[2], _ring3);

            Console.WriteLine($"Initial rotor positions: {_keyOffset[0]}, {_keyOffset[1]}, {_keyOffset[2]}");
            ROTORdisplay(lblRing1, _ring1);
            ROTORdisplay(lblRing2, _ring2);
            ROTORdisplay(lblRing3, _ring3);
            DisplayOffset();
        }

        private bool ValidateRotorInput(string input, int rotorIndex)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show($"Rotor {rotorIndex + 1}: Please enter a number between 0 and 25.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(input, out int value))
            {
                MessageBox.Show($"Rotor {rotorIndex + 1}: Input must be a valid number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (value < 0 || value > 25)
            {
                MessageBox.Show($"Rotor {rotorIndex + 1}: Number must be between 0 and 25.", "Invalid Range", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _initOffset[rotorIndex] = value; 
            return true;
        }



        private string InitializeRotors(int initial, string ring)
        {
            string newRing = ring;
            for (int x = 0; x < initial; x++)
                newRing = MoveValues(true, newRing);
            return newRing;
        }

        private string RemoveLastLetter(string input)
        {
            if (input.Length > 0)
            {
                return input.Substring(0, input.Length - 1);
            }
            return "";
        }

        private void txtBRing1Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing1Init.Text = "";
        }

        private void txtBRing2Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing2Init.Text = "";
        }

        private void txtBRing3Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing3Init.Text = "";
        }

        private void DisplayOffset()
        {
            txtBRing1Init.Text = _keyOffset[0].ToString(); 
            txtBRing2Init.Text = _keyOffset[1].ToString();
            txtBRing3Init.Text = _keyOffset[2].ToString();
        }

        private void SetupPlugboard(string plugboardSettings)
        {
            _plugboard.Clear();
            string[] pairs = plugboardSettings.ToUpper().Split(' ');
            foreach (string pair in pairs)
            {
                if (pair.Length == 2)
                {
                    _plugboard[pair[0]] = pair[1];
                    _plugboard[pair[1]] = pair[0]; 
                }
            }
        }

        private void btnSetPlugboard_Click(object sender, RoutedEventArgs e)
        {
            if (_plugboardSet)
            {
                MessageBox.Show("Plugboard is already set.");
                return;
            }

            SetupPlugboard(txtPlugboard.Text);
            _plugboardSet = true;
            btnRotor.IsEnabled = true;

            if (_plugboardSet)
            {
                txtPlugboard.IsEnabled = false;
            }
        }

        private void txtPlugboard_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtInput.Text = "";
            txtEncrypt.Text = "";
            txtEncryptMirror.Text = "";
        }

        private void txtBRing3Init_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        
    }
}

