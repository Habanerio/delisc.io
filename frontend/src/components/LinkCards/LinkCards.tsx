import React from 'react';

import LinkCard from './LinkCard';
import { LinkItem } from '@/models/LinkItem';

type LinkCardsProps = {
   items: LinkItem[] | undefined;
};

export default function LinkCards({ items = [] }: LinkCardsProps) {
   const getLinkCards = (): React.ReactNode => {
      return items.map((link: LinkItem, idx: number) => {
         // Images near the top should have a 'high' priority
         const priority = idx < 6;

         return <LinkCard key={link.id} {...link} />;
      });
   };

   return (
      <>
         <div className='links-list grid grid-cols-4 gap-4'>{getLinkCards()}</div>
      </>
   );
}
