import sys
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import math

def is_diff(a, b, thresh=15):
    return 1 if abs(a - b) > thresh else 0

def is_diff_cos(a, b, thresh=30):
    cos_thresh = math.cos(thresh/180*3.14)
    cos_sim = math.cos(math.radians(a - b))
    return 0 if cos_sim > cos_thresh else 1


def main(path1, path2):
    #labels = ["x", "y", "z", "w"]
    labels = ["x", "y", "z"]
    data1 = pd.read_csv(path1, header=None, names=labels)
    data2 = pd.read_csv(path2, header=None, names=labels)
    data1 = np.array(data1)
    data2 = np.array(data2)
    diff_each_list = []
    diff_list = []
    diff_x = []
    diff_y = []
    diff_z = []
    diff_each = [diff_x, diff_y, diff_z]
    diff_sum = 0
    if len(data1) > len(data2):
        data1 = data1[:len(data2)]
    else:
        data2 = data2[:len(data1)]
    for d1, d2 in zip(data1, data2):
        sum_each_time = 0
        thresh = 15
        x = is_diff_cos(d1[0], d2[0], thresh=thresh)
        y = is_diff_cos(d1[1], d2[1], thresh=thresh)
        z = is_diff_cos(d1[2], d2[2], thresh=thresh)

        diff_x.append(x)
        diff_y.append(y)
        diff_z.append(z)
        sum_each_time += 1 if x + y + z >= 1 else 0
        diff_each_list.append(sum_each_time)
        diff_sum += sum_each_time
        diff_list.append(diff_sum)

    fig = plt.figure()
    ax = []
    for i in range(len(labels)+1):
        ax.append(fig.add_subplot(len(labels)+1, 1, (i+1)))
    for i, label in enumerate(labels):
        d1 = list(data1[:,i])
        d2 = list(data2[:,i])
        d1 = np.array(d1)
        d2 = np.array(d2)
        d3 = np.array(diff_each[i])
        x = np.arange(len(d1))

        ax[i].plot(x, d1, label="sensor")
        ax[i].plot(x, d2, label="stored")
        ax[i].set_ylim(-10.0, 370.0)
        #ax[i].plot(x, d3, label="diff")
        axa = ax[i].twinx()
        axa.set_ylim(0.0, 5.0)
        axa.plot(x, d3, label="diff", color="g")

        ax[i].legend()
    print("similarlity:", 1 - (diff_sum/len(diff_list)))
    x_diff = np.arange(len(diff_list))
    ax[len(ax)-1].set_ylim(-5, len(diff_list) + 5)
    for i in range(len(diff_each_list)):
        diff_each_list[i] *= len(diff_each_list)
    ax[len(ax)-1].plot(x_diff, diff_list, label="diff sum")
    ax[len(ax)-1].plot(x_diff, diff_each_list, label="diff each time")
    ax[len(ax)-1].legend()
    plt.show()


if __name__ == "__main__":
    path1 = sys.argv[1]
    path2 = sys.argv[2]
    main(path1, path2)
