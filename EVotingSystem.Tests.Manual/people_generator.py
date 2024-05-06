from faker import Faker
import json

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


candidates = []
voters = []

fake = Faker('pl_PL')
for _ in range(10):
    candidate = Person(fake.email(), fake.phone_number(), fake.name())
    candidates.append(candidate)

voters = candidates.copy()
for _ in range(188):
    voter = Person(fake.email(), fake.phone_number(), fake.name())
    voters.append(voter)

with open('candidates.json', 'w', encoding='utf8') as json_file:
    json.dump([p.__json__() for p in candidates], json_file, ensure_ascii=False, indent=2)

with open('voters.json', 'w', encoding='utf8') as json_file:
    json.dump([p.__json__() for p in voters], json_file, ensure_ascii=False, indent=2)

