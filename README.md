# SocialChoiceNeuralNetwork
## Contents
  - NNVotingController program for controlling Neural Networks for social choices
  - Former outputs used for analysis

## Program
### Requirements
  - PYthon 2.7+
  - .NET Framework 4.5 or higher
  - Microsoft Office Excel 2007 or newer with macros enabled

### Configuration
  - Set the path of your Python executable (pythonw.exe) in the pythonpath.config file (default is the default install path of Python)

### How to use
#### General
  - The program can be started with the Run.vbs script
  - Outputs are created in the Results folder

#### Train Neural Network
  - Runs the Neural Network creating Python code (nn_training.py) with the selected inputs
  - The created neural netowrks are stored in files with the .network extension
  - The list shows the generated input files (the ones with numbers as extensions) that are found in the Results folder (See Generate Main Input)
  - The random seed of the training can be set in the upper left corner. Selecting multiple seeds will result in multiple .network files

#### Use Neural Network
  - Uses the pre-created .network files to evaluate the profiles in the .noncond files and give an estimated winner for every profile
  - The upper list shows the generated input files that are found in the Results folder (See Generate Main Input)
  - The lower list shows the generated network files that are found in the Results folder (See Train Neural Network)
  - Multiple selection is allowed in both lists. If multiple options are selected, then every possible combination is evaluated. Impossible combinations (ex.: 3 nominee in input file, but 5 nominee in network) are simply skipped.
  - The results of every combination are saved in different files with the .results extension.

#### Generate Main Input
  - This module creates voting profiles according to the settings on the page
  - Running this module will create two input files for the neural netowrks. The input file of the neural network training will include every profile that satisfies the selected types from the samples. These files will have the number of nominees selected as their extension. The rest of the profiles will be saved in a file with the extension .noncond
  - A profile satisfies a selected type if a clear winner exists
  - The Allow different ordering of voters switch will allow the random generation of the profiles to include the same profile, but with a different ordeing of the voters. The Repetition setting is only enabled if this switch is on. If Repetition is not zero every profile will be repeated according to the number but the order of voters will be randomly mixed for every repetition. This will increase the final sample size in a multiplicative way.
  - The Description and Use Time Stamp options only influence the output filanames to help sort the results.
  - The disabled coding settings are outdated and not supported by the last statistics module. The binary coding takes the  upper triangle of the pairwise comparsion matrix of every voter for all of the nomninees. The matrix is transformed into a vector, and the results are written in the files. The normal version stores the choices of every single voter, while the SUM version only stores the summed pairwise comparison matrix of the voters for every profile.
  - The neural networks input is based on the length of a profile row. This means that SUM version is significantly faster, and the run speed does not increase with the number of voters.

#### Generate Unanimity Input
  - This module is meant to test whether the neural netowrk can learn the unanimity property
  - This module creates voting profiles according to the settings on the page. The main settings work like the ones in the Main Input
  - Every profile is generated with an unanimity winner and every profile is saved in a file.
  - The type selection decides whether a training file (includes winner) or an evaluation file (does not include winner) or both are created
  
#### Generate Pareto Input
  - This module is meant to test whether the neural netowrk can learn the pareto dominance property
  - This module creates voting profiles according to the settings on the page. The main settings work like the ones in the Main Input
  - Every profile is generated with a pareto dominated nominee and every profile is saved in a file.
  - The type selection decides whether a training file (includes winner) or an evaluation file (does not include winner) or both are created
  - The algorithm calculating the results when the Allow different ordering of voters is off is poorly optimized. It's use is not advised

#### Create XLS files from results
  - TODO

## Outputs
  - TODO
