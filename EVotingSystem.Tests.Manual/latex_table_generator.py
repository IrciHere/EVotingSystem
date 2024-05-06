import json
import csv

# all votes
file_path_votes = "votes.json"

with open(file_path_votes, "r", encoding='utf-8') as json_file:
    votes = json.load(json_file)

latex_all_votes = ""

for vote in votes:
    row = f"{vote['email']}&{vote['candidateId']}&TAK \\\\\n"
    latex_all_votes += row

print(latex_all_votes)
all_votes_f = open("all_votes_latex.txt", "w")
all_votes_f.write(latex_all_votes)
all_votes_f.close()


# sum results
file_path_results = "sum_results.csv"

election_results = []

with open(file_path_results, mode ='r')as file:
    csvFile = csv.DictReader(file)
    for line in csvFile:
        election_results.append(line)

latex_results = ""

for result in election_results:
    row = f"{result['candidateid']}&{result['email']}&{result['votes']}\\\\\n"
    latex_results += row

results_f = open("results_latex.txt", "w")
results_f.write(latex_results)
results_f.close()

