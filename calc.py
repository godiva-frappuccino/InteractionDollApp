import sys
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import math

def main(path1, path2):
    #labels = ["x", "y", "z", "w"]
    labels = ["x", "y", "z"]
    data1 = pd.read_csv(path1, header=None, names=labels)
    data2 = pd.read_csv(path2, header=None, names=labels)

    data1 = np.array(data1)
    data2 = np.array(data2)
    diff_list = []
    diff_sum = 0
    for d1, d2 in zip(data1, data2):
        sum_each_time = 0
        sum_each_time += math.cos(math.radians(d1[0] - d2[0]));
        sum_each_time += math.cos(math.radians(d1[1] - d2[1]));
        sum_each_time += math.cos(math.radians(d1[2] - d2[2]));
        diff_sum += sum_each_time
        #diff_list.append(diff_sum)
        diff_list.append(sum_each_time)

    fig = plt.figure()
    ax = []
    for i in range(len(labels)+1):
        ax.append(fig.add_subplot(len(labels)+1, 1, (i+1)))
    for i, label in enumerate(labels):
        print(label)
        d1 = list(data1[:,i])
        d2 = list(data2[:,i])
        print(len(d1), len(d2))
        while(len(d1) < len(d2)):
            d1.append(0.0)
        while(len(d1) > len(d2)):
            d2.append(0.0)
        print(len(d1), len(d2))
        d1 = np.array(d1)
        d2 = np.array(d2)
        x = np.arange(len(d1))
        print(d1)

        ax[i].plot(x, d1, label=label)
        ax[i].plot(x, d2, label=label)
        ax[i].legend()
    x_diff = np.arange(len(diff_list))
    ax[len(ax)-1].plot(x_diff, diff_list, label="diff")
    plt.show()


if __name__ == "__main__":
    path1 = sys.argv[1]
    path2 = sys.argv[2]
    main(path1, path2)
