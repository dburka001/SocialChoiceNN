import sys
import win32_unicode_argv
import csv
import os
from numpy import *

#-----------------------------------------------------------------------
# Parameters
resultPath = sys.argv[1]
inputFileName = sys.argv[2]
networkFileName = sys.argv[3]
output_format = '{:0.0f}' # Correct: '{:0.0f}' - Others only for testing - Test example: '{:0.5f}'
#-----------------------------------------------------------------------

# MLP Forward function
def mlpfwd(inputs):
    # Getting Neural Network data
    isBeta = False
    isWeights1 = False
    isWeights2 = False
    weights1 = []
    weights2 = []
    with open(resultPath + networkFileName + '.network', 'rb') as f:
        reader = csv.reader(f, delimiter=';')
        for line in reader:            
            if not line:
                isBeta = False
                isWeights1 = False
                isWeights2 = False
            elif isBeta:
                beta = int(float(line[0]))
            elif isWeights1:
                line = [float(i) for i in line]
                weights1.append(asarray(line))
            elif isWeights2:
                line = [float(i) for i in line]
                weights2.append(asarray(line))
            elif line[0] == 'beta':
                isBeta = True
            elif line[0] == 'weights1':
                isWeights1 = True
            elif line[0] == 'weights2':
                isWeights2 = True
            elif line[0] == 'Outputs:':
                output_size = int(line[1])

    weights1 = asarray(weights1)
    weights2 = asarray(weights2)

    # MLP Fowrward functions
    hidden = dot(inputs, weights1);
    hidden = 1.0/(1.0+exp(-beta*hidden))
    hidden = concatenate((hidden,-ones((shape(inputs)[0],1))),axis=1)
    outputs = dot(hidden,weights2);
    normalisers = sum(exp(outputs),axis=1)*ones((1,shape(outputs)[0]))
    return transpose(transpose(exp(outputs))/normalisers)

# Read CSV
data_non_cond = []

with open(resultPath + inputFileName + '.noncond', 'rb') as f:
    reader = csv.reader(f, delimiter=';')
    for line in reader:
        line = [float(i) for i in line]
        data_non_cond.append(asarray(line))
data_non_cond = asarray(data_non_cond)

# MLP
data_non_cond_mlp = data_non_cond
data_non_cond_mlp = concatenate((data_non_cond_mlp,-ones((shape(data_non_cond_mlp)[0],1))),axis=1)
data_non_cond_mlp = mlpfwd(data_non_cond_mlp)
data_non_cond_mlp = where(data_non_cond_mlp>0.5,1,0) # Round method, so we get 0-1 values in the end. The original code has this too!
output_data = concatenate((data_non_cond, data_non_cond_mlp), axis=1)
with open(resultPath + inputFileName + '_X_' + networkFileName + '.result', 'wb') as csvfile:
    writer = csv.writer(csvfile, delimiter=';')    
    for row in output_data:
        writer.writerow([output_format.format(x) for x in row])
print ''
print 'MLP non condorcet results saved'

