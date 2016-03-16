import sys
import win32_unicode_argv
import csv
import os
from numpy import *

#-----------------------------------------------------------------------
# Parameters
resultPath = sys.argv[1]
filename = sys.argv[2] # Filename (without extension) of condorcet training data
output_size = int(float(sys.argv[3])) # Number of columns of the output
SEED = int(float(sys.argv[4]))
set_size = [2.0, 1.0, 1.0] # Size of training, validation and test data
run = [True, False] # What modules to run [MLP RBF]
run_non_cond = [True, False] # What result modules to run [MLP RBF] -  Depeneds on the variable "run"
output_format = '{:0.0f}' # Correct: '{:0.0f}' - Others only for testing - Test example: '{:0.5f}'
save_results = True
#MLP
nhidden = 5 # Number of hidden neurons
eta = 0.001 # eta
#RBF
nRBF = 50 # Number of RBF neurons
#-----------------------------------------------------------------------

# Read CSV
data = []

with open(resultPath + filename + '.' + str(output_size), 'rb') as f:
    reader = csv.reader(f, delimiter=';')
    for line in reader:
        line = [float(i) for i in line]
        data.append(asarray(line))
data = asarray(data)

row = shape(data)[0] # Number of rows in data
col = shape(data)[1] # Number of columns in data

# Set target
target = data[:,col-output_size:col]

# Randomly order the data
random.seed(SEED)
order = range(row)
random.shuffle(order)
data = data[order,:]
target = target[order,:]

# Split into training, validation, and test sets
set_size = [round(row * x / sum(set_size), 0) for x in set_size]
for i in range(len(set_size)):
    if i > 0:
        set_size[i] = set_size[i] + set_size[i - 1]

train = data[:set_size[0],0:col-output_size]
traint = target[:set_size[0]]
valid = data[set_size[0]:set_size[1],0:col-output_size]
validt = target[set_size[0]:set_size[1]]
test = data[set_size[1]:set_size[2],0:col-output_size]
testt = target[set_size[1]:set_size[2]]

# Train the network
# MLP
if run[0]:
    import mlp as mlp_module
    mlp = mlp_module.mlp(train,traint,nhidden,outtype='softmax')
    mlp.earlystopping(train,traint,valid,validt,eta)
    mlp_correct_pct = mlp.confmat(test,testt)
    if save_results:
        mlp.get_data(resultPath, filename, SEED)
        print 'MLP weights saved'

#RBF
if run[1]:
    import rbf as rbf_module
    rbf = rbf_module.rbf(train,traint,nRBF,1,1)
    rbf.rbftrain(train,traint,0.25,2000)
    rbf_correct_pct = rbf.confmat(test,testt)
    if save_results:
        rbf.get_data(resultPath + filename)
        print 'RBF weights saved'
