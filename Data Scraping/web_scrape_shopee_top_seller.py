from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import TimeoutException
from webdriver_manager.chrome import ChromeDriverManager
from datetime import datetime
from random import randint
from time import sleep
import pandas as pd

# create object for chrome options
chrome_options = Options()
chrome_options.add_argument('disable-notifications')
chrome_options.add_argument('--disable-infobars')
chrome_options.add_argument("--incognito")
chrome_options.add_argument('start-maximized')
chrome_options.add_argument("disable-infobars")
chrome_options.add_experimental_option("prefs", {"profile.default_content_setting_values.notifications": 2})
chrome_options.add_argument('user-data-dir=C:\\Users\\User\\AppData\\Local\\Google\\Chrome\\User Data\\Default')

webpage_number = 0


def primary_thread():
    user_search_word = input("Search: ")
    search_shopee_price(user_search_word)
    sleep(2)


def check_is_null_data(data):
    if data is None:
        data = ''
    else:
        data = data.text.strip()
    return data


def get_shopee_url(page_number, shopee_search_word, search_word):
    if page_number == 0:
        """Generate an url from the search term"""
        template = "https://shopee.ph/search?keyword="
        search_word = search_word.replace(' ', '%20')
        shopee_search_word = template + search_word

    # add page query placeholder
    shopee_url = shopee_search_word + '&page=' + str(page_number)
    return shopee_url, shopee_search_word


def get_shopee_price_list(soup):
    page_price_list_data = []
    for item in soup.find_all('div', {'class': 'col-xs-2-4 shopee-search-item-result__item'}):

        item_name = item.find('div', {'class': 'ie3A+n bM+7UW Cve6sh'})
        if item_name is not None:
            item_name = item_name.text.strip()
        else:
            item_name = ''

        item_price = item.find('div', {'class': 'vioxXd rVLWG6'})
        if item_price is not None:
            item_price = item_price.find('span', {'class': 'ZEgDH9'}).text.strip().replace(',', '')
        else:
            item_price = ''

        item_discount = item.find('div', {'class': 'NTmuqd _3NQO+7 WVxeBE'})
        if item_discount is not None:
            item_discount = item_discount.find('span', {'class': 'percent'}).text.strip()
        else:
            item_discount = ''

        item_sold = item.find('div', {'class': 'r6HknA uEPGHT'})
        item_sold = check_is_null_data(item_sold)

        item_location = item.find('div', {'class': 'zGGwiV'})
        item_location = check_is_null_data(item_location)

        item_initial_price = item.find('div', {'class': 'vioxXd ZZuLsr d5DWld'})
        item_initial_price = check_is_null_data(item_initial_price).replace('₱', '')

        # print(name, initial_price, discount, price, sold, location)
        page_price_list_data.append(
            [item_name, item_initial_price, item_discount, item_price, item_sold, item_location])
        # print(item_name, item_initial_price, item_discount, item_price, item_sold, item_location)

    return page_price_list_data


def search_shopee_price(search_word):
    shopee_data = []
    shopee_url, shopee_search_word = get_shopee_url(0, '', search_word)
    print("Scraping Data from : " + shopee_url)

    browser = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)
    browser.get(shopee_url)
    delay = 15

    try:
        WebDriverWait(browser, delay)
        sleep(2)

        total_height = int(browser.execute_script("return document.body.scrollHeight"))
        for i in range(1, total_height, 5):
            browser.execute_script("window.scrollTo(0, {});".format(i))

        length_of_page = browser.execute_script(
            "window.scrollTo(0, document.body.scrollHeight);"
            "var lenOfPage=document.body.scrollHeight;return lenOfPage;")
        match = False
        while not match:
            last_count = length_of_page
            sleep(3)
            length_of_page = browser.execute_script(
                "window.scrollTo(0, document.body.scrollHeight);"
                "var lenOfPage=document.body.scrollHeight;return lenOfPage;")
            if last_count == length_of_page:
                match = True

        sleep(3)
        html = browser.execute_script("return document.getElementsByTagName('html')[0].innerHTML")
        soup = BeautifulSoup(html, "html.parser")
        page_controller = soup.find('div', {'class': 'shopee-mini-page-controller__state'})

        if not page_controller:  # results not found
            print("No results found")
            print("Try different or more general keywords")
            browser.quit()
            sleep(2)
            primary_thread()

        current_page = int(page_controller.find('span',
                                                {'class': 'shopee-mini-page-controller__current'}).text.strip()) - 1
        max_page = int(page_controller.find('span',
                                            {'class': 'shopee-mini-page-controller__total'}).text.strip()) - 1
        if max_page > 4:
            max_page = 4

        price_data_list = get_shopee_price_list(soup)
        shopee_data.extend(price_data_list)
        browser.quit()

        while current_page < max_page:
            browser.quit()
            current_page += 1
            shopee_url, shopee_search_word = get_shopee_url(current_page, shopee_search_word, search_word)
            print("Scraping Data from : " + shopee_url)

            browser = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)
            browser.get(shopee_url)
            delay = 15

            try:
                WebDriverWait(browser, delay)
                sleep(2)

                total_height = int(browser.execute_script("return document.body.scrollHeight"))
                for i in range(1, total_height, 5):
                    browser.execute_script("window.scrollTo(0, {});".format(i))

                length_of_page = browser.execute_script(
                    "window.scrollTo(0, document.body.scrollHeight);"
                    "var lenOfPage=document.body.scrollHeight;return lenOfPage;")
                match = False
                while not match:
                    last_count = length_of_page
                    sleep(3)
                    length_of_page = browser.execute_script(
                        "window.scrollTo(0, document.body.scrollHeight);"
                        "var lenOfPage=document.body.scrollHeight;return lenOfPage;")
                    if last_count == length_of_page:
                        match = True

                sleep(3)
                html = browser.execute_script("return document.getElementsByTagName('html')[0].innerHTML")
                soup = BeautifulSoup(html, "html.parser")
                price_data_list = get_shopee_price_list(soup)
                shopee_data.extend(price_data_list)

            except TimeoutException:
                print("Loading took too much time!-Try again")

            x = randint(1, 10)
            sleep(x)
            browser.close()
    except TimeoutException:
        print("Loading took too much time!-Try again")
        browser.quit()
        sleep(2)
        primary_thread()

    browser.quit()

    scraped_data = pd.DataFrame(shopee_data, columns=['Item Name', 'Initial Price', 'Discount', 'Price', 'Sold',
                                                      'Location'])
    scraped_data.to_excel(r'C:\\Grocery Pricing\\' + datetime.today().strftime("%m-%d-%y_") + search_word +
                          '_price_list.xlsx', index=False, header=True)
    print("Data Saved to " + r'C:\\Grocery Pricing\\' + datetime.today().strftime("%m-%d-%y_") + search_word +
          '_price_list.xlsx')
    primary_thread()


if __name__ == '__main__':
    primary_thread()
