from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route('/sms.do', methods=['POST'])
def send_sms():
    to = request.args.get('to')
    message = request.args.get('message')

    print(f"SMS na numer +{to}: '{message}'")

    return jsonify({'success': True}), 200

if __name__ == '__main__':
    app.run(debug=True, port=5024)