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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constants for rotor wiring and reflector (as in your code)
        string _control = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string _ring1 = "DMTWSILRUYQNKFEJCAZBPGXOHV";
        string _ring2 = "HQZGPJTMOBLNCIFDYAWVEUSRKX";
        string _ring3 = "UQNTLSZFMREHDPXKIBVYGJCWOA";
        string _reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";

        // Rotor offset tracking
        int[] _keyOffset = { 0, 0, 0 };
        int[] _initOffset = { 0, 0, 0 };

        // Rotor state flag
        bool _rotor = false;

        // Plugboard setup
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

            // Attach click event handlers to all letter buttons
            LetterQ.Click += LetterButton_Click;
            LetterW.Click += LetterButton_Click;
            LetterE.Click += LetterButton_Click;
            LetterR.Click += LetterButton_Click;
            LetterT.Click += LetterButton_Click;
            LetterY.Click += LetterButton_Click;
            LetterU.Click += LetterButton_Click;
            LetterI.Click += LetterButton_Click;
            LetterO.Click += LetterButton_Click;
            LetterP.Click += LetterButton_Click;
            LetterA.Click += LetterButton_Click;
            LetterS.Click += LetterButton_Click;
            LetterD.Click += LetterButton_Click;
            LetterF.Click += LetterButton_Click;
            LetterG.Click += LetterButton_Click;
            LetterH.Click += LetterButton_Click;
            LetterJ.Click += LetterButton_Click;
            LetterK.Click += LetterButton_Click;
            LetterL.Click += LetterButton_Click;
            LetterZ.Click += LetterButton_Click;
            LetterX.Click += LetterButton_Click;
            LetterC.Click += LetterButton_Click;
            LetterV.Click += LetterButton_Click;
            LetterB.Click += LetterButton_Click;
            LetterN.Click += LetterButton_Click;
            LetterM.Click += LetterButton_Click;
            ButtonBackspace.Click += ButtonBackspace_Click;
            ButtonSpace.Click += ButtonSpace_Click;

            // Attach GotFocus event handlers to your textboxes
            txtInput.GotFocus += TextBox_GotFocus;
            txtPlugboard.GotFocus += TextBox_GotFocus;
            txtBRing1Init.GotFocus += TextBox_GotFocus;
            txtBRing2Init.GotFocus += TextBox_GotFocus;
            txtBRing3Init.GotFocus += TextBox_GotFocus;
        }

        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null)
            {
                Button clickedButton = (Button)sender;
                _activeTextBox.Text += clickedButton.Content.ToString();
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length; //Move caret to end
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _activeTextBox = (TextBox)sender;
        }

        private void DisplayRing(Label displayLabel, string ring)
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
                    return index;
                }
            }
            return index;
        }


        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = txtInput.Text.ToUpper();

            if (input.Length < _previousInputLength)
            {
                // Handle deletion (no rotor rotation)
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
                    txtEncrypt.Text += " "; // Add space without encryption
                    txtEncryptMirror.Text += " ";
                }
                else if (char.IsLetter(newChar))
                {
                    // Encrypt letter and mirror
                    txtEncrypt.Text += Encrypt(newChar);
                    txtEncryptMirror.Text += Mirror(newChar);

                    // Rotate rotors after encryption
                    if (_rotor)
                    {
                        Rotate(true);
                    }
                }
                else
                {
                    // Remove invalid input and show warning
                    txtInput.Text = RemoveLastLetter(input);
                    txtInput.CaretIndex = txtInput.Text.Length;
                    MessageBox.Show("Only letters and spaces are allowed!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            // Update previous length after processing
            _previousInputLength = txtInput.Text.Length;
        }

        private char Encrypt(char letter)
        {
            char newChar = letter;

            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

            newChar = _ring3[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];
            newChar = _reflector[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring3[IndexSearch(_control, newChar)];

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

            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

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

            DisplayRing(lblControlRing, _control);
            DisplayRing(lblRing1, _ring1);
            DisplayRing(lblRing2, _ring2);
            DisplayRing(lblRing3, _ring3);
            DisplayRing(ReflectorLabel, _reflector);
        }

        private void Rotate(bool forward)
        {
            if (forward)
            {
                _keyOffset[2]++;
                _ring3 = MoveValues(forward, _ring3);

                if (_keyOffset[2] >= _control.Length)
                {
                    _keyOffset[2] = 0;
                    _keyOffset[1]++;
                    _ring2 = MoveValues(forward, _ring2);

                    if (_keyOffset[1] >= _control.Length)
                    {
                        _keyOffset[1] = 0;
                        _keyOffset[0]++;
                        _ring1 = MoveValues(forward, _ring1);

                        if (_keyOffset[0] >= _control.Length)
                        {
                            _keyOffset[0] = 0;
                        }
                    }
                }
            }
            else
            {
                if (_keyOffset[2] > 0 || _keyOffset[1] > 0)
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

            Console.WriteLine($"Rotor positions: {_keyOffset[0]}, {_keyOffset[1]}, {_keyOffset[2]}"); // Added log
            DisplayRing(lblRing1, _ring1);
            DisplayRing(lblRing2, _ring2);
            DisplayRing(lblRing3, _ring3);
            DisplayOffset();
        }

        private string MoveValues(bool forward, string ring)
        {
            if (forward)
                return ring.Substring(1) + ring[0];
            else
                return ring[ring.Length - 1] + ring.Substring(0, ring.Length - 1);
        }


        // Handle rotor button click
        private void btnRotor_Click(object sender, RoutedEventArgs e)
        {
            SetDefaults();

            // Validate all inputs
            if (!ValidateRotorInput(txtBRing1Init.Text, 0) ||
                !ValidateRotorInput(txtBRing2Init.Text, 1) ||
                !ValidateRotorInput(txtBRing3Init.Text, 2))
            {
                return; // Stop if any validation fails
            }

            // Lock settings and initialize rotors
            txtBRing1Init.IsEnabled = false;
            txtBRing2Init.IsEnabled = false;
            txtBRing3Init.IsEnabled = false;

            _rotor = true;
            btnRotor.Content = "Settings Lock";

            _ring1 = InitializeRotors(_initOffset[0], _ring1);
            _ring2 = InitializeRotors(_initOffset[1], _ring2);
            _ring3 = InitializeRotors(_initOffset[2], _ring3);

            Console.WriteLine($"Initial rotor positions: {_keyOffset[0]}, {_keyOffset[1]}, {_keyOffset[2]}");
            DisplayRing(lblRing1, _ring1);
            DisplayRing(lblRing2, _ring2);
            DisplayRing(lblRing3, _ring3);
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

            _initOffset[rotorIndex] = value; // Set valid offset
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
            txtBRing1Init.Text = _keyOffset[0] + "";
            txtBRing2Init.Text = _keyOffset[1] + "";
            txtBRing3Init.Text = _keyOffset[2] + "";
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

        private void ButtonSpace_Click(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null)
            {
                _activeTextBox.Text += " ";
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length; // Move caret to end
            }
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (_activeTextBox != null && _activeTextBox.Text.Length > 0)
            {
                _activeTextBox.Text = _activeTextBox.Text.Substring(0, _activeTextBox.Text.Length - 1);
                _activeTextBox.CaretIndex = _activeTextBox.Text.Length; // Move caret to end
            }
        }
    }
}

