import sys
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

def main(path):
    #labels = ["x", "y", "z", "w"]
    labels = ["x", "y", "z"]
    data = pd.read_csv(path, header=None, names=labels)
    print(data.head())
    x = np.arange(len(data))
    for label in labels:
        plt.plot(x, data[label], label=label)
    plt.legend()
    plt.show()

if __name__ == "__main__":
    path = sys.argv[1]
    main(path)
