#!/usr/bin/python3
# Script to get umami stats and events for opalopc.com
# reads umami username and password from envs
# username = UMAMI_USERNAME
# password = UMAMI_PASSWORD


from urllib import request
import json
import time
import sys
import os

class UmamiClient:

    _token = None
    _website_id = "e8c79fbd-9bdd-4f68-92a3-61b93464a836" # opalopc.com website uid on umami

    def __init__(self, username, password):
        self.authenticate(username, password)
        pass

    def authenticate(self, username, password):
        body = request.urlopen(request.Request(
            'https://umami.molemmat.fi/api/auth/login',
            headers={'Content-Type': 'application/json'},
            data=json.dumps({
                'username': username,
                'password': password
            }).encode())).read()
        self.token = json.loads(body)["token"]

    def get_data(self, startAt, endAt):
        return f"""Stats:
{self._get_stats(startAt, endAt)}

Metrics (uri):
{self._get_metrics_uri(startAt, endAt)}

Metrics (referrer):
{self._get_metrics_referrer(startAt, endAt)}

Events:
{self._get_events(startAt, endAt)}

Run User-Agents:
{self._get_run_user_agents(startAt, endAt)}
"""

    def _get_stats(self, startAt, endAt):
        return self._prettify_json(request.urlopen(request.Request(
            f'https://umami.molemmat.fi/api/websites/{self._website_id}/stats?startAt={startAt}&endAt={endAt}',
            headers={'Content-Type': 'application/json', 'Authorization': f'Bearer {self.token}'},
            )).read())

    def _get_metrics_uri(self, startAt, endAt):
        return self._prettify_json(request.urlopen(request.Request(
            f'https://umami.molemmat.fi/api/websites/{self._website_id}/metrics?startAt={startAt}&endAt={endAt}&type=url',
            headers={'Content-Type': 'application/json', 'Authorization': f'Bearer {self.token}'},
            )).read())

    def _get_metrics_referrer(self, startAt, endAt):
        return self._prettify_json(request.urlopen(request.Request(
            f'https://umami.molemmat.fi/api/websites/{self._website_id}/metrics?startAt={startAt}&endAt={endAt}&type=referrer',
            headers={'Content-Type': 'application/json', 'Authorization': f'Bearer {self.token}'},
            )).read())

    def _get_events(self, startAt, endAt):
        return self._prettify_json(request.urlopen(request.Request(
            f'https://umami.molemmat.fi/api/websites/{self._website_id}/events?unit=hour&timezone=Europe/Helsinki&startAt={startAt}&endAt={endAt}',
            headers={'Content-Type': 'application/json', 'Authorization': f'Bearer {self.token}'},
            )).read())

    def _get_run_user_agents(self, startAt, endAt):
        return self._prettify_json(request.urlopen(request.Request(
            f'https://umami.molemmat.fi/api/event-data/fields?websiteId={self._website_id}&startAt={startAt}&endAt={endAt}&field=user-agent',
            headers={'Content-Type': 'application/json', 'Authorization': f'Bearer {self.token}'},
            )).read())


    def _prettify_json(self, jsonString):
        return json.dumps(json.loads(jsonString), indent=4)

def current_milli_time():
    return round(time.time() * 1000)



if __name__ == "__main__":

    if (len(sys.argv) <= 1):
        print("Usage: get-umami-metrics.py <number of days to check metrics for>")
        exit(1)

    ndays = int(sys.argv[1])

    client = UmamiClient(os.environ['UMAMI_USERNAME'], os.environ['UMAMI_PASSWORD'])
    msnow = current_milli_time()
    msago = msnow - ndays*24*60*60*1000

    print(client.get_data(msago, msnow))
