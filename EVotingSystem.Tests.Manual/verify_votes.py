import requests
import csv
import json

votes_otps = []

with open('hash_with_otp.csv', mode ='r')as file:
    csvFile = csv.reader(file)
    for line in csvFile:
        votes_otps.append(line)

file_path = "votes_with_hash.json"

with open(file_path, "r", encoding='utf-8') as json_file:
    votes = json.load(json_file)

for vote in votes:
    otp_code = next(a for a in votes_otps if a[0] == vote['voteHash'])
    vote['otpCode'] = otp_code[1]

login_url = "https://localhost:7202/login"
common_password = "1q2@3e4R"
validate_vote_url = "https://localhost:7202/Votes/validate-vote"

for vote in votes[:-3]:
    print(vote['email'])
    jwt_token = requests.post(login_url, json={'email': vote['email'], 'password': common_password}, verify=False).text
    headers = {"Authorization": f"Bearer {jwt_token}"}
    requests.post(validate_vote_url, headers=headers, json={'electionId': 1, 'voteHash': vote['voteHash'], 'otpCode': vote['otpCode']}, verify=False)