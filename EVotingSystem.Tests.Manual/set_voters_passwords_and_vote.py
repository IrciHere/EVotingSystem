import csv
import json
import requests

class Person:
    def __init__(self, email, phone_number, name) -> None:
        self.email = email
        self.phone_number = phone_number if phone_number.startswith("+48") else f"+48{phone_number}"
        self.name = name[name.index(" ") + 1:] if (name.startswith("pan ") or name.startswith("pani ")) else name
    
    def __json__(self):
        return {
            "name": self.name,
            "phoneNumber": self.phone_number.replace(" ", ""),
            "email": self.email
        }


def from_json(json_obj):
    return Person(json_obj['email'], json_obj['phoneNumber'], json_obj['name'])


voters_reset_codes = []

with open('reset_codes.csv', mode ='r')as file:
    csvFile = csv.reader(file)
    for line in csvFile:
        voters_reset_codes.append(line)

file_path = "votes.json"

with open(file_path, "r", encoding='utf-8') as json_file:
    votes = json.load(json_file)

for vote in votes:
    reset_code = next(a for a in voters_reset_codes if a[0] == vote['email'])
    vote['resetCode'] = reset_code[1]

reset_password_url = "https://localhost:7202/reset-password"
login_url = "https://localhost:7202/login"
common_password = "1q2@3e4R"
vote_url = "https://localhost:7202/Votes"

for vote in votes:
    requests.patch(reset_password_url, json={'resetCode': vote['resetCode'], 'newPassword': common_password}, verify=False)
    jwt_token = requests.post(login_url, json={'email': vote['email'], 'password': common_password}, verify=False).text
    headers = {"Authorization": f"Bearer {jwt_token}"}
    response_hash = requests.put(vote_url, headers=headers, json={'electionId': 1, 'candidateId': vote['candidateId'], 'voterPassword': common_password}, verify=False).text
    vote['voteHash'] = response_hash

with open('votes_with_hash.json', 'w', encoding='utf8') as json_file:
    json.dump(votes, json_file, ensure_ascii=False, indent=2)
