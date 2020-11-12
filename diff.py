import sys
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

def main(path1, path2):
    #labels = ["x", "y", "z", "w"]
    labels = ["x", "y", "z"]
    data1 = pd.read_csv(path1, header=None, names=labels)
    data2 = pd.read_csv(path2, header=None, names=labels)
    x = np.arange(len(data1))
    y = np.arange(len(data2))
    fig = plt.figure()
    ax = []
    #ax = [fig.add_subplot(len(1, len(labels), i) for i in range(len(labels)))]
    for i in range(len(labels)):
        ax.append(fig.add_subplot(1, len(labels), (i+1)))
    for i, label in enumerate(labels):
        print(label)
        d1 = list(data1[label])
        d2 = list(data2[label])
        while(len(d1) < len(d2)):
            d1.append(0.0)
        while(len(d1) > len(d2)):
            d2.append(0.0)
        ax[i].plot(x, d1, label=label)
        ax[i].plot(x, d2, label=label)
        ax[i].legend()
    plt.show()

if __name__ == "__main__":
    path1 = sys.argv[1]
    path2 = sys.argv[2]
    main(path1, path2)
