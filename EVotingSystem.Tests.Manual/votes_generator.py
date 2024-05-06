import json
import random

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


file_path = "voters.json"

with open(file_path, "r", encoding='utf-8') as json_file:
    voters = json.load(json_file, object_hook=from_json)

votes = []

for voter in voters:
    vote = {'email': voter.email, 'candidateId': random.randint(1, 10)}
    votes.append(vote)

with open('votes.json', 'w', encoding='utf8') as json_file:
    json.dump(votes, json_file, ensure_ascii=False, indent=2)